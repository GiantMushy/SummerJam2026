using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Streams the endless world around the player. The map is fixed-height (64 tiles)
/// and infinite along X. Each column is a pure function of its absolute index + seed,
/// so terrain is deterministic and can be cleared/regenerated without storing a grid.
///
/// Layout per column: cliffs fill the top and bottom; a snaking open "corridor" runs
/// between them and occasionally crosses the vertical centre. Isolated cliff blobs drop
/// into the corridor to force weaving. Every few columns the corridor is force-opened at
/// the centre row so the player can always reach the Eclipse line.
///
/// Root-level singleton, mirroring GameManager / CameraManager / EclipseController — not
/// part of the three-tier character hierarchy.
/// </summary>
public class EndlessWorldManager : MonoBehaviour
{
    public static EndlessWorldManager instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap dirtMap;
    [SerializeField] private Tilemap grassMap;
    [SerializeField] private Tilemap cliffMap;   // the "World Border" tilemap (has the TilemapCollider2D)

    [Header("Tiles (RuleTiles)")]
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase cliffTile;

    [Header("World dimensions")]
    [SerializeField] private int worldBottomY = -32;  // lowest tile row; confirm against painted art
    [SerializeField] private int worldHeight = 64;    // total rows
    [SerializeField] private int borderMin = 1;       // cliff rows always kept at top & bottom

    [Header("Corridor")]
    [SerializeField] private float baseGapWidth = 22f;  // open height at difficulty 0
    [SerializeField] private int minGap = 6;            // hard floor on open height (car width + margin)
    [SerializeField] private float gapNoiseFreq = 0.045f;
    [SerializeField] private float gapNoiseAmp = 16f;   // how far the corridor wanders from centre

    [Header("Obstacles")]
    [SerializeField, Range(0f, 1f)] private float obstacleChance = 0.14f;
    [SerializeField] private int maxObstacleHeight = 6;
    [SerializeField] private int minPassage = 4;        // open rows left beside an obstacle

    [Header("Centre access (Eclipse)")]
    [SerializeField] private int centerCheckpointInterval = 60; // columns between guaranteed openings
    [SerializeField] private int checkpointWindow = 8;          // columns on each side that ease open

    [Header("Grass (cosmetic)")]
    [SerializeField, Range(0f, 1f)] private float grassFringeChance = 0.5f;
    [SerializeField] private int grassMaxDepth = 3;
    [SerializeField] private float grassPatchFreq = 0.18f;
    [SerializeField, Range(0f, 1f)] private float grassPatchThreshold = 0.74f;

    [Header("Streaming")]
    [SerializeField] private int generateRadiusWest = 60;  // player drives west, lead this edge further
    [SerializeField] private int generateRadiusEast = 32;
    [SerializeField] private int clearBuffer = 12;

    [Header("Difficulty")]
    [SerializeField] private float difficultyDistance = 3000f; // westward distance at which difficulty ~= 1

    [Header("Floating origin")]
    [SerializeField] private float rebaseThreshold = 2000f; // rebase when |player.x| exceeds this
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private EclipseController eclipse;

    [Header("Seed")]
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private int seed = 12345;

    // Salts keep the different deterministic decisions decorrelated from one another.
    const uint SaltObstacle = 0x1111u;
    const uint SaltObstacleSize = 0x2222u;
    const uint SaltObstacleAnchor = 0x3333u;
    const uint SaltGrassFringe = 0x4444u;

    private Transform playerTransform;
    private Rigidbody2D playerRb;

    private int minGeneratedX;          // inclusive cell-X range currently painted
    private int maxGeneratedX;
    private int worldOffsetColumns;     // absoluteColumn = cellX + worldOffsetColumns
    private uint seedU;
    private float noiseOffsetX, noiseOffsetY, noiseOffsetX2, noiseOffsetY2, patchOffsetX, patchOffsetY;

    private int CenterRow => worldBottomY + worldHeight / 2;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this); return; }
    }

    void Start()
    {
        GameObject player = GameManager.instance.player; // hard ref; errors loudly if unassigned
        playerTransform = player.transform;
        playerRb = player.GetComponent<Rigidbody2D>();

        if (randomizeSeed) seed = Random.Range(int.MinValue, int.MaxValue);
        seedU = (uint)seed;
        InitNoiseOffsets();

        ResetAndGenerate();
    }

    void Update()
    {
        UpdateStreaming();
    }

    void FixedUpdate()
    {
        MaybeRebase();
    }

    // ---- Seeding -----------------------------------------------------------

    private void InitNoiseOffsets()
    {
        // PerlinNoise mirrors around integers and is zero at the origin, so push each
        // sample into a seed-dependent, non-integer region of the noise field.
        System.Random r = new(seed);
        noiseOffsetX = (float)r.NextDouble() * 1000f;
        noiseOffsetY = (float)r.NextDouble() * 1000f;
        noiseOffsetX2 = (float)r.NextDouble() * 1000f;
        noiseOffsetY2 = (float)r.NextDouble() * 1000f;
        patchOffsetX = (float)r.NextDouble() * 1000f;
        patchOffsetY = (float)r.NextDouble() * 1000f;
    }

    // ---- Streaming ---------------------------------------------------------

    private void UpdateStreaming()
    {
        int p = Mathf.FloorToInt(playerTransform.position.x);
        EnsureGenerated(p - generateRadiusWest, p + generateRadiusEast);
        ClearOutside(p - generateRadiusWest - clearBuffer, p + generateRadiusEast + clearBuffer);
    }

    /// <summary>Extends the painted range outward to cover [from, to].</summary>
    private void EnsureGenerated(int from, int to)
    {
        while (minGeneratedX > from) GenerateColumn(--minGeneratedX);
        while (maxGeneratedX < to) GenerateColumn(++maxGeneratedX);
    }

    /// <summary>Clears painted columns that fall outside [from, to].</summary>
    private void ClearOutside(int from, int to)
    {
        while (minGeneratedX < from) ClearColumn(minGeneratedX++);
        while (maxGeneratedX > to) ClearColumn(maxGeneratedX--);
    }

    private void ResetAndGenerate()
    {
        dirtMap.ClearAllTiles();
        grassMap.ClearAllTiles();
        cliffMap.ClearAllTiles();

        int p = Mathf.FloorToInt(playerTransform.position.x);
        minGeneratedX = maxGeneratedX = p;
        GenerateColumn(p);
        EnsureGenerated(p - generateRadiusWest, p + generateRadiusEast);
    }

    // ---- Per-column generation --------------------------------------------

    private void GenerateColumn(int cellX)
    {
        int absCol = cellX + worldOffsetColumns;
        ComputeCorridor(absCol, out int gapBottom, out int gapTop);
        ComputeObstacle(absCol, gapBottom, gapTop, out int obsBottom, out int obsTop);

        for (int row = 0; row < worldHeight; row++)
        {
            int y = worldBottomY + row;
            bool cliff = y < gapBottom || y > gapTop || (y >= obsBottom && y <= obsTop);
            Vector3Int cell = new(cellX, y, 0);

            dirtMap.SetTile(cell, dirtTile);
            cliffMap.SetTile(cell, cliff ? cliffTile : null);
            bool grass = !cliff && ShouldGrass(absCol, y, gapBottom, gapTop);
            grassMap.SetTile(cell, grass ? grassTile : null);
        }
    }

    private void ClearColumn(int cellX)
    {
        for (int row = 0; row < worldHeight; row++)
        {
            Vector3Int cell = new(cellX, worldBottomY + row, 0);
            dirtMap.SetTile(cell, null);
            cliffMap.SetTile(cell, null);
            grassMap.SetTile(cell, null);
        }
    }

    // ---- Corridor shape ----------------------------------------------------

    private void ComputeCorridor(int absCol, out int gapBottom, out int gapTop)
    {
        float diff = ColumnDifficulty(absCol);

        float wander = Mathf.PerlinNoise(absCol * gapNoiseFreq + noiseOffsetX, noiseOffsetY) - 0.5f;
        float center = CenterRow + wander * 2f * gapNoiseAmp * (0.5f + 0.5f * diff);

        float wobble = (Mathf.PerlinNoise(absCol * gapNoiseFreq * 2.3f + noiseOffsetX2, noiseOffsetY2) - 0.5f) * 4f;
        float width = Mathf.Lerp(baseGapWidth, minGap + 2f, diff) + wobble;
        width = Mathf.Max(width, minGap);

        ApplyCenterCheckpoint(absCol, ref center, ref width);

        int half = Mathf.RoundToInt(width * 0.5f);
        int c = Mathf.RoundToInt(center);
        gapBottom = c - half;
        gapTop = c + half;
        ClampGapToBorders(ref gapBottom, ref gapTop);
    }

    /// <summary>Keeps the gap inside the world and never thinner than minGap, so a column is never sealed.</summary>
    private void ClampGapToBorders(ref int gapBottom, ref int gapTop)
    {
        int minY = worldBottomY + borderMin;
        int maxY = worldBottomY + worldHeight - 1 - borderMin;

        gapBottom = Mathf.Clamp(gapBottom, minY, maxY);
        gapTop = Mathf.Clamp(gapTop, minY, maxY);

        if (gapTop - gapBottom < minGap)
        {
            gapTop = Mathf.Min(gapBottom + minGap, maxY);
            gapBottom = gapTop - minGap;
        }
    }

    /// <summary>Near a checkpoint column, ease the corridor back to the centre row and widen it.</summary>
    private void ApplyCenterCheckpoint(int absCol, ref float center, ref float width)
    {
        if (centerCheckpointInterval <= 0) return;

        int phase = ((absCol % centerCheckpointInterval) + centerCheckpointInterval) % centerCheckpointInterval;
        int dist = Mathf.Min(phase, centerCheckpointInterval - phase);
        if (dist >= checkpointWindow) return;

        float t = 1f - (float)dist / checkpointWindow;
        center = Mathf.Lerp(center, CenterRow, t);
        width = Mathf.Max(width, Mathf.Lerp(width, baseGapWidth, t));
    }

    // ---- Obstacles ---------------------------------------------------------

    /// <summary>
    /// Optionally drops a single-column cliff blob anchored to one edge of the corridor,
    /// leaving a contiguous passage of >= minPassage on the other side. Obstacles are kept
    /// isolated (no obstacle in either neighbour column) so a full-gap neighbour always
    /// overlaps the passage — guaranteeing an unbroken east-west route.
    /// </summary>
    private void ComputeObstacle(int absCol, int gapBottom, int gapTop, out int obsBottom, out int obsTop)
    {
        obsBottom = 1; obsTop = 0; // empty (bottom > top means "no obstacle")

        if (!HasObstacle(absCol)) return;
        if (HasObstacle(absCol - 1) || HasObstacle(absCol + 1)) return;

        int gap = gapTop - gapBottom + 1;
        int maxH = Mathf.Min(maxObstacleHeight, gap - minPassage);
        if (maxH < 1) return;

        int h = 1 + Mathf.FloorToInt(H(absCol, SaltObstacleSize) * maxH);
        bool anchorTop = H(absCol, SaltObstacleAnchor) < 0.5f;

        if (anchorTop) { obsTop = gapTop; obsBottom = gapTop - h + 1; }
        else { obsBottom = gapBottom; obsTop = gapBottom + h - 1; }
    }

    private bool HasObstacle(int absCol) => H(absCol, SaltObstacle) < obstacleChance;

    // ---- Grass -------------------------------------------------------------

    private bool ShouldGrass(int absCol, int y, int gapBottom, int gapTop)
    {
        int depth = Mathf.Min(y - gapBottom, gapTop - y); // 0 = touching a cliff edge
        if (depth < grassMaxDepth)
        {
            float chance = grassFringeChance * (1f - depth / (float)grassMaxDepth);
            if (H(absCol, y, SaltGrassFringe) < chance) return true;
        }

        float patch = Mathf.PerlinNoise(absCol * grassPatchFreq + patchOffsetX, y * grassPatchFreq + patchOffsetY);
        return patch > grassPatchThreshold;
    }

    // ---- Difficulty --------------------------------------------------------

    /// <summary>0 at the start, ramping to 1 as columns run west (-X). Deterministic per column.</summary>
    private float ColumnDifficulty(int absCol)
    {
        float westDistance = Mathf.Max(0f, -absCol);
        return Mathf.Clamp01(westDistance / difficultyDistance);
    }

    // ---- Floating origin ---------------------------------------------------

    /// <summary>
    /// When the player drifts too far from the origin, translate the whole live world
    /// (player, AI, camera, Eclipse) back toward it and re-key terrain so coordinates stay
    /// small and FP32 physics stays precise. Everything moves by the same delta, so nothing
    /// visibly shifts on screen.
    /// </summary>
    private void MaybeRebase()
    {
        if (playerRb == null) return;

        float px = playerRb.position.x;
        if (Mathf.Abs(px) <= rebaseThreshold) return;

        int shift = Mathf.RoundToInt(px);     // move everything by -shift => player returns near x = 0
        Vector2 delta = new(-shift, 0f);

        playerRb.position += delta;
        AiManager.instance?.ShiftAll(delta);
        WorldSpawnManager.instance?.ShiftAll(delta);
        if (cameraManager != null) cameraManager.transform.position += (Vector3)delta;
        if (eclipse != null) eclipse.transform.position += (Vector3)delta;

        // Keep each cell's absolute column constant so the regenerated terrain is identical
        // and lands at the same on-screen position as before the shift.
        worldOffsetColumns += shift;
        ResetAndGenerate();
    }

    // ---- Spawn support (used by WorldSpawnManager) -------------------------

    /// <summary>Absolute, rebase-invariant column index for a world X.</summary>
    public int WorldXToAbsoluteColumn(float worldX) => Mathf.FloorToInt(worldX) + worldOffsetColumns;

    /// <summary>Current cell-X (grid coordinate, ~world X) for an absolute column.</summary>
    public int AbsoluteColumnToCellX(int absCol) => absCol - worldOffsetColumns;

    /// <summary>
    /// Returns a drivable, never-on-a-cliff spawn point in the given column. The vertical
    /// target is centreRow + offsetFromCenter, clamped into the open corridor and pushed off
    /// any obstacle blob — so by construction it lands on open ground. A final cliff-tile check
    /// guards against any drift between the analytic shape and the painted tiles.
    /// </summary>
    public bool TryGetSpawnPoint(int absCol, float offsetFromCenter, out Vector2 worldPoint)
    {
        worldPoint = default;

        ComputeCorridor(absCol, out int gapBottom, out int gapTop);
        ComputeObstacle(absCol, gapBottom, gapTop, out int obsBottom, out int obsTop);

        int lo = gapBottom + 1;
        int hi = gapTop - 1;
        if (hi < lo) { lo = gapBottom; hi = gapTop; } // degenerate gap fallback

        int targetY = Mathf.Clamp(Mathf.RoundToInt(CenterRow + offsetFromCenter), lo, hi);
        targetY = PushOffObstacle(targetY, lo, hi, obsBottom, obsTop);

        int cellX = AbsoluteColumnToCellX(absCol);
        if (cliffMap.GetTile(new Vector3Int(cellX, targetY, 0)) != null
            && !TryFindClearRow(cellX, lo, hi, out targetY))
            return false;

        worldPoint = new Vector2(cellX + 0.5f, targetY + 0.5f); // tile centre
        return true;
    }

    /// <summary>Moves a row out of an obstacle blob to the nearer open side of the corridor.</summary>
    private int PushOffObstacle(int y, int lo, int hi, int obsBottom, int obsTop)
    {
        if (obsBottom > obsTop) return y;       // no obstacle in this column
        if (y < obsBottom || y > obsTop) return y; // already clear of it

        int below = obsBottom - 1;
        int above = obsTop + 1;
        bool belowOk = below >= lo;
        bool aboveOk = above <= hi;

        if (belowOk && (!aboveOk || (y - below) <= (above - y))) return below;
        if (aboveOk) return above;
        return y; // shouldn't happen given the minPassage guarantee
    }

    /// <summary>Finds the first row in [lo, hi] whose cliff cell is empty.</summary>
    private bool TryFindClearRow(int cellX, int lo, int hi, out int y)
    {
        for (int yy = lo; yy <= hi; yy++)
        {
            if (cliffMap.GetTile(new Vector3Int(cellX, yy, 0)) == null) { y = yy; return true; }
        }
        y = lo;
        return false;
    }

    // ---- Hash helpers ------------------------------------------------------

    private float H(int x, uint salt) => WorldGenUtil.Hash01(x, seedU ^ salt);
    private float H(int x, int y, uint salt) => WorldGenUtil.Hash01(x, y, seedU ^ salt);
}
