using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central tick for every AI agent. Instead of 500+ MonoBehaviours each paying Unity's
/// per-instance Update/FixedUpdate message overhead, agents register here and are driven
/// from a single loop. Mirrors the GameManager / InputManager singleton pattern.
/// </summary>
public class AiManager : MonoBehaviour
{
    public static AiManager instance;

    [Tooltip("Agents farther than this from the player are destroyed (endless streaming). 0 disables culling.")]
    [SerializeField] private float aiDespawnDistance = 60f;

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

    /// <summary>
    /// Translates every registered agent by <paramref name="delta"/>. Used by the
    /// floating-origin rebase so AI stay aligned with the world when it shifts toward origin.
    /// </summary>
    public void ShiftAll(Vector2 delta)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            Rigidbody2D rb = agents[i].rb;
            if (rb != null) rb.position += delta;
            else agents[i].transform.position += (Vector3)delta;
        }
    }

    void Update()
    {
        CullFar();
        for (int i = 0; i < agents.Count; i++)
            agents[i].TickUpdate();
    }

    /// <summary>
    /// Destroys agents the player has left far behind so the endless world doesn't
    /// accumulate AI forever. Attached/chasing agents stay near the player and survive.
    /// </summary>
    private void CullFar()
    {
        if (aiDespawnDistance <= 0f || GameManager.instance == null) return;
        GameObject player = GameManager.instance.player;
        if (player == null) return;

        Vector2 p = player.transform.position;
        float sqr = aiDespawnDistance * aiDespawnDistance;

        // Backward so Despawn (which unregisters via OnDisable) doesn't disturb earlier indices.
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            Vector2 pos = agents[i].transform.position;
            if ((pos - p).sqrMagnitude > sqr)
                AiPool.instance.Despawn(agents[i]);
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < agents.Count; i++)
            agents[i].TickFixedUpdate();
    }
}
