using System;
using UnityEngine;

public class AiStatsManager : StatsManager
{
    public event Action TriggerDeath;

    private AiCharacterManager entity;
    private enum AiState { Idle, Chase, Attached, Dying }

    [Header("AI Specific Stats")]
    [SerializeField] private AiState currentState = AiState.Idle;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
        entity.aiInteractionManager.TriggerChase      += ActivateChaseState;
        entity.aiInteractionManager.TriggerAttached   += ActivateAttackState;
        entity.aiInteractionManager.TriggerLeaveRange += DeactivateChaseState;
    }

    private void OnDestroy()
    {
        entity.aiInteractionManager.TriggerChase      -= ActivateChaseState;
        entity.aiInteractionManager.TriggerAttached   -= ActivateAttackState;
        entity.aiInteractionManager.TriggerLeaveRange -= DeactivateChaseState;
    }

    protected override void SetInitialStats()
    {
        base.SetInitialStats();
    }

    // Ai can be idle, then chase is triggered when player gets close.
    // If player gets far enough away, it goes back to idle.
    // If player gets close enough (collision), it goes to attack state.
    // If health reaches 0, it goes to dying state.
    public void ActivateChaseState()  => currentState = AiState.Chase;
    public void ActivateAttackState() => currentState = AiState.Attached;

    public void ActivateDyingState()
    {
        currentState = AiState.Dying;
        TriggerDeath?.Invoke();
    }

    public void DeactivateChaseState() => currentState = AiState.Idle;

    public bool IsChasing() => currentState == AiState.Chase || currentState == AiState.Attached;
}
