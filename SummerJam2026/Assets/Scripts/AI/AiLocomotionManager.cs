using UnityEngine;

public class AiLocomotionManager : LocomotionManager
{
    private AiCharacterManager entity;
    private Transform playerTransform;
    private Vector2 directionToPlayer;
    private bool isChasing = false;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance.player != null)
            playerTransform = GameManager.instance.player.transform;
        entity.aiInteractionManager.TriggerChase      += OnChaseTriggered;
        entity.aiInteractionManager.TriggerAttached   += OnChaseTriggered;
        entity.aiInteractionManager.TriggerLeaveRange += OnLeaveRange;
    }

    private void OnDestroy()
    {
        entity.aiInteractionManager.TriggerChase      -= OnChaseTriggered;
        entity.aiInteractionManager.TriggerAttached   -= OnChaseTriggered;
        entity.aiInteractionManager.TriggerLeaveRange -= OnLeaveRange;
    }

    private void OnChaseTriggered()
    {
        currentSpeed = entity.aiStatsManager.GetMaxSpeed();
        isChasing = true;
    }

    private void OnLeaveRange()
    {
        isChasing = false;
        currentSpeed = 0f;
        entity.rb.linearVelocity = Vector2.zero;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void HandleDirectionalChange()
    {
        // Raycast or direct vector to player to determine direction
        if (!isChasing) return;
        directionToPlayer = ((Vector2)(playerTransform.position - transform.position)).normalized;
    }

    protected override void HandleMovement()
    {
        if (!isChasing) return;

        // Rotate towards player and move in that direction
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        entity.rb.MoveRotation(angle);
        entity.rb.linearVelocity = directionToPlayer * currentSpeed;
    }

    protected override void HandleCharacterPhysics() { }
}