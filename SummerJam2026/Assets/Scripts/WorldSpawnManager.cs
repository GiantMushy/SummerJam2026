using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Populates the endless world ahead of the westward-driving player with AI and pickups,
/// and destroys pickups left far behind. Works in absolute-column space (rebase-invariant)
/// so terrain regeneration / floating-origin rebases never duplicate spawns. Entities are
/// placed only in drivable corridor space via <see cref="EndlessWorldManager.TryGetSpawnPoint"/>,
/// never on mountains.
///
/// Root-level singleton, mirroring EndlessWorldManager / GameManager — not part of the
/// three-tier character hierarchy.
/// </summary>
public class WorldSpawnManager : MonoBehaviour
{
    public static WorldSpawnManager instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject fuelPickupPrefab;
    [SerializeField] private GameObject healthPickupPrefab;

    [Header("Spawn rates (probability per column)")]
    [SerializeField, Range(0f, 1f)] private float aiDensity = 0.04f;
    [SerializeField, Range(0f, 1f)] private float fuelFrequency = 0.03f;
    [SerializeField, Range(0f, 1f)] private float healthDensity = 0.02f;

    [Header("Vertical spread from centre (~ average distance in tiles)")]
    [SerializeField] private float aiCenterSpread = 14f;
    [SerializeField] private float fuelCenterSpread = 6f;
    [SerializeField] private float healthCenterSpread = 12f;

    [Header("Streaming (keep spawnAheadColumns < EndlessWorldManager.generateRadiusWest)")]
    [SerializeField] private int spawnAheadColumns = 45;   // off-screen lead in travel direction (west)
    [SerializeField] private int spawnAheadEast = 10;      // small lead behind (east)
    [SerializeField] private int initialSafeColumns = 24;  // columns around the start that never spawn

    [Header("Despawn")]
    [Tooltip("Pickups farther than this (horizontal) from the player are destroyed.")]
    [SerializeField] private float pickupDespawnDistance = 60f;

    private Transform playerTransform;
    private int minProcessedAbs;   // inclusive absolute-column range already rolled for spawns
    private int maxProcessedAbs;
    private readonly List<GameObject> pickups = new();

    private bool hasGaussianSpare;
    private float gaussianSpare;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this); return; }
    }

    void Start()
    {
        playerTransform = GameManager.instance.player.transform; // hard ref; errors loudly if unassigned

        // Mark the area around the start as already processed so nothing spawns on the player.
        int p = EndlessWorldManager.instance.WorldXToAbsoluteColumn(playerTransform.position.x);
        minProcessedAbs = p - initialSafeColumns;
        maxProcessedAbs = p + initialSafeColumns;
    }

    void Update()
    {
        int p = EndlessWorldManager.instance.WorldXToAbsoluteColumn(playerTransform.position.x);

        // Roll spawns for newly revealed columns. The frontier only grows outward, so driving
        // back and forth never re-spawns a region.
        while (minProcessedAbs > p - spawnAheadColumns) ProcessColumn(--minProcessedAbs);
        while (maxProcessedAbs < p + spawnAheadEast) ProcessColumn(++maxProcessedAbs);

        CullPickups();
    }

    // ---- Spawning ----------------------------------------------------------

    private void ProcessColumn(int absCol)
    {
        if (Random.value < aiDensity)
            TrySpawnAi(absCol, aiCenterSpread);
        if (fuelPickupPrefab != null && Random.value < fuelFrequency)
            TrySpawnPickup(fuelPickupPrefab, absCol, fuelCenterSpread);
        if (healthPickupPrefab != null && Random.value < healthDensity)
            TrySpawnPickup(healthPickupPrefab, absCol, healthCenterSpread);
    }

    // AI come from the pool (reused, not Instantiated) and manage their own lifecycle via
    // AiManager's cull / AiPool, so they aren't tracked here.
    private void TrySpawnAi(int absCol, float spread)
    {
        float offset = Gaussian(spread);
        if (EndlessWorldManager.instance.TryGetSpawnPoint(absCol, offset, out Vector2 point))
            AiPool.instance.Spawn(point);
    }

    private void TrySpawnPickup(GameObject prefab, int absCol, float spread)
    {
        float offset = Gaussian(spread);
        if (!EndlessWorldManager.instance.TryGetSpawnPoint(absCol, offset, out Vector2 point))
            return;

        pickups.Add(Instantiate(prefab, point, Quaternion.identity));
    }

    // ---- Despawn -----------------------------------------------------------

    private void CullPickups()
    {
        float px = playerTransform.position.x;
        for (int i = pickups.Count - 1; i >= 0; i--)
        {
            GameObject obj = pickups[i];
            if (obj == null) { SwapRemove(i); continue; } // collected by the player
            if (Mathf.Abs(obj.transform.position.x - px) > pickupDespawnDistance)
            {
                Destroy(obj);
                SwapRemove(i);
            }
        }
    }

    private void SwapRemove(int i)
    {
        int last = pickups.Count - 1;
        pickups[i] = pickups[last];
        pickups.RemoveAt(last);
    }

    /// <summary>Shifts tracked pickups by the floating-origin rebase delta (called by EndlessWorldManager).</summary>
    public void ShiftAll(Vector2 delta)
    {
        for (int i = 0; i < pickups.Count; i++)
            if (pickups[i] != null) pickups[i].transform.position += (Vector3)delta;
    }

    // ---- Helpers -----------------------------------------------------------

    /// <summary>Normally-distributed offset, mean 0, standard deviation <paramref name="stdDev"/> (Box–Muller).</summary>
    private float Gaussian(float stdDev)
    {
        if (hasGaussianSpare)
        {
            hasGaussianSpare = false;
            return stdDev * gaussianSpare;
        }

        float u1 = 1f - Random.value; // (0, 1] avoids log(0)
        float u2 = 1f - Random.value;
        float mag = Mathf.Sqrt(-2f * Mathf.Log(u1));
        gaussianSpare = mag * Mathf.Sin(2f * Mathf.PI * u2);
        hasGaussianSpare = true;
        return stdDev * (mag * Mathf.Cos(2f * Mathf.PI * u2));
    }
}
