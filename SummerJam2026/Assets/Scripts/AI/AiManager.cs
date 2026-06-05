using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central tick for every AI agent. Instead of 500+ MonoBehaviours each paying Unity's
/// per-instance Update/FixedUpdate message overhead, agents register here and are driven
/// from a single loop. Mirrors the GameManager / InputManager singleton pattern.
/// </summary>
public class AiManager : MonoBehaviour
{
    private static AiManager instance;

    /// <summary>The current instance without creating one (null if none exists). Use for cleanup.</summary>
    public static AiManager existing => instance;

    /// <summary>
    /// Lazily resolves the manager so agents can register regardless of scene setup or
    /// Awake/OnEnable ordering. Reuses a scene-placed instance if one exists, otherwise
    /// creates one on demand.
    /// </summary>
    public static AiManager instanceOrCreate
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AiManager>();
                if (instance == null)
                    instance = new GameObject(nameof(AiManager)).AddComponent<AiManager>();
            }
            return instance;
        }
    }

    private readonly List<AiCharacterManager> agents = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(this); return; }
    }

    public void Register(AiCharacterManager agent)
    {
        agents.Add(agent);
    }

    public void Unregister(AiCharacterManager agent)
    {
        // Swap-back removal keeps this O(1) at 500+ agents (order doesn't matter here).
        int index = agents.IndexOf(agent);
        if (index < 0) return;
        int last = agents.Count - 1;
        agents[index] = agents[last];
        agents.RemoveAt(last);
    }

    void Update()
    {
        for (int i = 0; i < agents.Count; i++)
            agents[i].TickUpdate();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < agents.Count; i++)
            agents[i].TickFixedUpdate();
    }
}
