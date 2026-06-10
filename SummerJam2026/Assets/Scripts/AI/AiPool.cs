using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reuses a fixed set of AI instances instead of Instantiate/Destroy churn. The player drives
/// endlessly, streaming AI in (spawn) and out (death / distance cull) constantly; pooling
/// activates an idle instance on spawn and deactivates it on return, removing per-spawn
/// allocations and the GC spikes from Destroy.
///
/// Root-level singleton, mirroring AiManager / WorldSpawnManager — not part of the three-tier
/// character hierarchy. Owns the AI prefab so all creation funnels through here.
/// </summary>
public class AiPool : MonoBehaviour
{
    public static AiPool instance;

    [SerializeField] private GameObject aiPrefab;            // Plant Default
    [Tooltip("Inactive instances pre-created at startup so the first wave of spawns doesn't hitch.")]
    [SerializeField] private int prewarmCount = 16;
    [Tooltip("Container the idle instances are parented under. Defaults to this transform.")]
    [SerializeField] private Transform poolParent;

    private readonly Stack<AiCharacterManager> idle = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this); return; }

        if (poolParent == null) poolParent = transform;
    }

    void Start()
    {
        for (int i = 0; i < prewarmCount; i++)
            idle.Push(CreateInactive());
    }

    /// <summary>Activates an instance at <paramref name="position"/>, reset to a fresh-spawn state.</summary>
    public AiCharacterManager Spawn(Vector2 position)
    {
        AiCharacterManager agent = idle.Count > 0 ? idle.Pop() : CreateInactive();

        Transform t = agent.transform;
        t.SetParent(null);
        t.position = position;
        t.rotation = Quaternion.identity;

        agent.gameObject.SetActive(true);   // OnEnable re-subscribes + registers with AiManager
        agent.PrepareForSpawn();            // after activation so every manager's OnEnable has run
        return agent;
    }

    /// <summary>Deactivates an agent and returns it to the pool. Safe to call more than once.</summary>
    public void Despawn(AiCharacterManager agent)
    {
        if (agent == null) return;
        // Already returned — guards a death-Invoke racing a distance-cull on the same instance.
        if (!agent.gameObject.activeSelf) return;

        agent.gameObject.SetActive(false);  // OnDisable unsubscribes + unregisters from AiManager
        agent.transform.SetParent(poolParent);
        idle.Push(agent);
    }

    private AiCharacterManager CreateInactive()
    {
        GameObject obj = Instantiate(aiPrefab, poolParent);
        obj.SetActive(false);
        return obj.GetComponent<AiCharacterManager>();
    }
}
