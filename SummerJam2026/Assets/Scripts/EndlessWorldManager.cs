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
    [Tooltip("Ground tilemap. Filled solid across the play area with the dirt tile.")]
    [SerializeField] private Tilemap dirtMap;
    [Tooltip("Scenery tilemap for grass (mountain halo + scattered patches). No collision.")]
    [SerializeField] private Tilemap grassMap;
    [Tooltip("The 'World Border' tilemap that carries the TilemapCollider2D. All mountains/cliffs (borders + obstacle islands) are painted here, so they collide.")]
    [SerializeField] private Tilemap cliffMap;

    [Header("Tiles (RuleTiles)")]
    [Tooltip("RuleTile painted on every ground cell (dirtMap).")]
    [SerializeField] private TileBase dirtTile;
    [Tooltip("RuleTile painted on grass cells (grassMap).")]
    [SerializeField] private TileBase grassTile;
    [Tooltip("RuleTile painted on every mountain/cliff cell (cliffMap).")]
    [SerializeField] private TileBase cliffTile;

    [Header("World dimensions")]
    [Tooltip("Y coordinate of the lowest in-play tile row. Must match where the art/camera expect the bottom of the world. Higher = world sits higher in Y; lower = sits lower. Change only to align with the scene, not for gameplay.")]
    [SerializeField] private int worldBottomY = -32;
    [Tooltip("Total in-play height in tiles (vertical playfield). HIGH = taller world, more room to manoeuvre and wider possible corridor. LOW = short, claustrophobic world. The design target is 64.")]
    [SerializeField] private int worldHeight = 64;
    [Tooltip("Minimum mountain rows always kept at the top AND bottom (border thickness floor). HIGH = thicker frame, less vertical play space. LOW = thinner frame; the 2.5D tileset breaks below 5. Keep >= 5.")]
    [SerializeField] private int borderMin = 5;
    [Tooltip("Extra mountain rows painted ABOVE the play area so the 2.5D north cliff has body/top. HIGH = more off-screen cliff drawn above (safe, slightly more tiles). LOW (<4) = the north cliff top may look cut off.")]
    [SerializeField] private int northExtra = 4;
    [Tooltip("Extra mountain rows painted BELOW the play area for the 2.5D south cliff body. HIGH = more off-screen cliff below. LOW (<4) = the south cliff may look cut off.")]
    [SerializeField] private int southExtra = 4;
    [Tooltip("Feature granularity in columns: every mountain feature is at least this wide, killing 1-cell spikes. HIGH = chunkier, blockier terrain that steps less often. LOW (=1) = jagged 1-wide spikes the tileset can't render. Keep >= 2.")]
    [SerializeField] private int blockWidth = 2;

    [Header("Corridor")]
    [Tooltip("Open driving height (gap between top & bottom mountains) at difficulty 0 / the start. HIGH = wide, easy corridor. LOW = tight from the very start. Shrinks toward minGap as difficulty rises.")]
    [SerializeField] private float baseGapWidth = 22f;
    [Tooltip("Absolute floor on the open corridor height, in tiles — the corridor never gets thinner than this even at max difficulty, so it's never sealed. HIGH = always roomy (easier). LOW = can become very tight (harder). Must comfortably fit the car (~6).")]
    [SerializeField] private int minGap = 6;
    [Tooltip("How quickly the corridor snakes up/down along X. HIGH = rapid, twisty wiggles. LOW = long, gentle sweeps. Sampled per block, so ~2x the per-column value.")]
    [SerializeField] private float gapNoiseFreq = 0.09f;
    [Tooltip("How far the corridor centre wanders from the middle of the world, in tiles. HIGH = corridor swings far up/down and the mountains cross the centre often. LOW = corridor stays near the middle (straighter).")]
    [SerializeField] private float gapNoiseAmp = 16f;

    [Header("Obstacles (rectangular islands)")]
    [Tooltip("Probability (per ~2-column block) of spawning a rectangular cliff island in the corridor. HIGH = obstacles almost everywhere (very busy, hard). LOW / 0 = few or no islands; weaving comes only from the snaking corridor.")]
    [SerializeField, Range(0f, 1f)] private float obstacleChance = 0.14f;
    [Tooltip("Minimum island height in tiles. The 2.5D cliff needs >= 5. HIGH = only tall islands appear (and only where the corridor is tall enough). LOW (<5) = islands too short for the tileset to render correctly.")]
    [SerializeField] private int minObstacleHeight = 5;
    [Tooltip("Maximum island height in tiles. HIGH = islands can nearly span the corridor (very tight squeezes). LOW = only small islands. Clamped so a passage of minPassage always remains.")]
    [SerializeField] private int maxObstacleHeight = 8;
    [Tooltip("Open rows guaranteed beside an island (the drivable gap left around it). HIGH = islands stay small / generous room to pass. LOW = islands can choke the corridor down to a sliver. Keep it wide enough for the car.")]
    [SerializeField] private int minPassage = 4;

    [Header("Centre access (Eclipse)")]
    [Tooltip("Columns between guaranteed centre openings, so the player can periodically reach the Eclipse line. HIGH = rare centre access (harder to reach the Eclipse). LOW = the corridor is forced back to centre often (easier). 0 disables forced openings.")]
    [SerializeField] private int centerCheckpointInterval = 60;
    [Tooltip("Width in columns of each forced centre opening (how gradually it eases open/closed). HIGH = long, gentle funnels back to centre. LOW = abrupt, narrow openings.")]
    [SerializeField] private int checkpointWindow = 8;

    [Header("Grass patches (scattered scenery, in addition to the mountain halo)")]
    [Tooltip("Scale of the scattered grass blobs on open ground. HIGH = small, speckly tufts. LOW = large, smooth grassy regions.")]
    [SerializeField] private float grassPatchFreq = 0.18f;
    [Tooltip("Noise cutoff for placing a grass patch (0..1). HIGH = less grass (only the densest spots). LOW = more grass; near 0 covers almost everything. This is on top of the always-on mountain halo.")]
    [SerializeField, Range(0f, 1f)] private float grassPatchThreshold = 0.74f;

    [Header("Streaming")]
    [Tooltip("Columns generated ahead of the player to the WEST (the travel direction). HIGH = more world ready ahead (smoother, more CPU/memory). LOW = world may not be painted before it scrolls on screen. Keep WorldSpawnManager.spawnAheadColumns below this.")]
    [SerializeField] private int generateRadiusWest = 60;
    [Tooltip("Columns generated behind the player to the EAST. HIGH = more trailing world kept. LOW = world clears closer behind the player.")]
    [SerializeField] private int generateRadiusEast = 32;
    [Tooltip("Extra margin (columns) beyond the generate radius before a column is cleared, to avoid generate/clear thrashing at the edges. HIGH = smoother but more tiles kept alive. LOW = tighter memory, risk of flicker at the edges.")]
    [SerializeField] private int clearBuffer = 12;

    [Header("Difficulty")]
    [Tooltip("Westward distance (tiles) at which difficulty reaches ~1 (corridor tightest, most obstacles). HIGH = the game ramps up slowly over a long drive. LOW = it gets hard very quickly.")]
    [SerializeField] private float difficultyDistance = 3000f;

    [Header("Floating origin")]
    [Tooltip("When the player's |x| exceeds this, the whole world is shifted back toward the origin to keep float precision. HIGH = rebases rarely (coords grow larger before reset). LOW = rebases often (coords stay tiny). Mostly leave as-is; lower only to TEST the rebase.")]
    [SerializeField] private float rebaseThreshold = 2000f;
    [Tooltip("Camera to shift during a rebase. If unassigned the camera visibly slides for a few frames after each rebase.")]
    [SerializeField] private CameraManager cameraManager;
    [Tooltip("Eclipse to shift during a rebase so it stays aligned with the world.")]
    [SerializeField] private EclipseController eclipse;

    [Header("Seed")]
    [Tooltip("ON = a new random world every run. OFF = use the fixed Seed below (reproducible terrain — useful for testing/comparing).")]
    [SerializeField] private bool randomizeSeed = true;
    [Tooltip("Fixed world seed used when Randomize Seed is OFF. Same seed => identical terrain every run.")]
    [SerializeField] private int seed = 12345;

    // Salts keep the different deterministic decisions decorrelated from one another.
    const uint SaltObstacle = 0x1111u;
    const uint SaltObstacleSize = 0x2222u;
    const uint SaltObstacleAnchor = 0x3333u;

    private Transform playerTransform;
    private Rigidbody2D playerRb;

    private int minGeneratedX;          // inclusive cell-X range currently painted
    private int maxGeneratedX;
    private int worldOffsetColumns;     // absoluteColumn = cellX + worldOffsetColumns
    private uint seedU;
    private float noiseOffsetX, noiseOffsetY, noiseOffsetX2, noiseOffsetY2, patchOffsetX, patchOffsetY;

    private int CenterRow => worldBottomY + worldHeight / 2;
    private int WorldTopY => worldBottomY + worldHeight - 1;
    private int PaintBottomY => worldBottomY - southExtra;
    private int PaintTopY => WorldTopY + northExtra;

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

    /// <summary>The mountain extent of one column: borders outside [gapBottom, gapTop] plus an obstacle blob.</summary>
    private readonly struct ColumnMask
    {
        public readonly int gapBottom, gapTop, obsBottom, obsTop;
        public ColumnMask(int gb, int gt, int ob, int ot) { gapBottom = gb; gapTop = gt; obsBottom = ob; obsTop = ot; }
        public bool IsMountain(int y) => y < gapBottom || y > gapTop || (y >= obsBottom && y <= obsTop);
    }

    private ColumnMask ComputeMask(int absCol)
    {
        ComputeCorridor(absCol, out int gapBottom, out int gapTop);
        ComputeObstacle(absCol, gapBottom, gapTop, out int obsBottom, out int obsTop);
        return new ColumnMask(gapBottom, gapTop, obsBottom, obsTop);
    }

    private void GenerateColumn(int cellX)
    {
        int absCol = cellX + worldOffsetColumns;

        // Masks for the three columns let grass dilate horizontally (8-neighbour halo) without
        // recomputing corridor/obstacle per cell.
        ColumnMask left = ComputeMask(absCol - 1);
        ColumnMask mid = ComputeMask(absCol);
        ColumnMask right = ComputeMask(absCol + 1);

        for (int y = PaintBottomY; y <= PaintTopY; y++)
        {
            Vector3Int cell = new(cellX, y, 0);

            bool cliff = mid.IsMountain(y);
            cliffMap.SetTile(cell, cliff ? cliffTile : null);

            // Grass covers every mountain tile + a 1-tile halo (incl. diagonals), plus scattered patches.
            bool grass = IsMountainAround(left, mid, right, y) || IsGrassPatch(absCol, y);
            grassMap.SetTile(cell, grass ? grassTile : null);

            // Dirt only across the in-play rows (the extra border rows are off-screen backing).
            if (y >= worldBottomY && y <= WorldTopY)
                dirtMap.SetTile(cell, dirtTile);
        }
    }

    /// <summary>True if any of the 3 columns is mountain at y-1, y, or y+1 (8-neighbour dilation incl. self).</summary>
    private bool IsMountainAround(ColumnMask left, ColumnMask mid, ColumnMask right, int y)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            int yy = y + dy;
            if (left.IsMountain(yy) || mid.IsMountain(yy) || right.IsMountain(yy)) return true;
        }
        return false;
    }

    /// <summary>Scattered organic grass on open ground, independent of the mountains (low-frequency Perlin).</summary>
    private bool IsGrassPatch(int absCol, int y)
    {
        float patch = Mathf.PerlinNoise(absCol * grassPatchFreq + patchOffsetX, y * grassPatchFreq + patchOffsetY);
        return patch > grassPatchThreshold;
    }

    private void ClearColumn(int cellX)
    {
        for (int y = PaintBottomY; y <= PaintTopY; y++)
        {
            Vector3Int cell = new(cellX, y, 0);
            dirtMap.SetTile(cell, null);
            cliffMap.SetTile(cell, null);
            grassMap.SetTile(cell, null);
        }
    }

    // ---- Corridor shape ----------------------------------------------------

    /// <summary>Integer floor division (handles negatives), so block indices are continuous across the origin.</summary>
    private static int FloorDiv(int a, int b) => (a >= 0) ? a / b : -(((-a) + b - 1) / b);

    /// <summary>Block index for a column. All columns in a block share one corridor/obstacle shape (>= blockWidth wide).</summary>
    private int BlockIndex(int absCol) => FloorDiv(absCol, blockWidth);

    private void ComputeCorridor(int absCol, out int gapBottom, out int gapTop)
    {
        // Everything is keyed on the block, so both columns of a block get an identical gap
        // (no 1-cell steps). repCol drives difficulty/checkpoints at column granularity.
        int b = BlockIndex(absCol);
        int repCol = b * blockWidth;
        float diff = ColumnDifficulty(repCol);

        float wander = Mathf.PerlinNoise(b * gapNoiseFreq + noiseOffsetX, noiseOffsetY) - 0.5f;
        float center = CenterRow + wander * 2f * gapNoiseAmp * (0.5f + 0.5f * diff);

        float wobble = (Mathf.PerlinNoise(b * gapNoiseFreq * 2.3f + noiseOffsetX2, noiseOffsetY2) - 0.5f) * 4f;
        float width = Mathf.Lerp(baseGapWidth, minGap + 2f, diff) + wobble;
        width = Mathf.Max(width, minGap);

        ApplyCenterCheckpoint(repCol, ref center, ref width);

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
        int maxY = WorldTopY - borderMin;

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
    /// Optionally carves a rectangular cliff island (blockWidth wide x h tall, h >= minObstacleHeight)
    /// anchored to one corridor edge, leaving a contiguous passage of >= minPassage on the other side.
    /// Keyed on the block (so it's a clean rectangle) and isolated by empty neighbour blocks, so a
    /// full-gap neighbour always overlaps the passage — guaranteeing an unbroken east-west route.
    /// </summary>
    private void ComputeObstacle(int absCol, int gapBottom, int gapTop, out int obsBottom, out int obsTop)
    {
        obsBottom = 1; obsTop = 0; // empty (bottom > top means "no obstacle")

        int b = BlockIndex(absCol);
        if (!HasObstacleBlock(b)) return;
        if (HasObstacleBlock(b - 1) || HasObstacleBlock(b + 1)) return;

        int gap = gapTop - gapBottom + 1;
        int maxH = Mathf.Min(maxObstacleHeight, gap - minPassage);
        if (maxH < minObstacleHeight) return; // no room for a >=5 tall island plus passage

        int h = minObstacleHeight + Mathf.FloorToInt(H(b, SaltObstacleSize) * (maxH - minObstacleHeight + 1));
        h = Mathf.Min(h, maxH);
        bool anchorTop = H(b, SaltObstacleAnchor) < 0.5f;

        if (anchorTop) { obsTop = gapTop; obsBottom = gapTop - h + 1; }
        else { obsBottom = gapBottom; obsTop = gapBottom + h - 1; }
    }

    private bool HasObstacleBlock(int b) => H(b, SaltObstacle) < obstacleChance;

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
}
