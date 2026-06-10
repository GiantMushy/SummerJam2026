using UnityEngine;

public class AiLocomotionManager : LocomotionManager
{
    private enum MoveState { Idle, Chasing, WindUp, Jumping, Recovery, Attached, Dying }

    private AiCharacterManager entity;
    private Transform playerTransform;
    private Collider2D playerCollider;

    private Collider2D bodyCollider;
    private LayerMask originalExcludeLayers;

    private MoveState state = MoveState.Idle;
    private float timer = 0f;
    private Vector2 directionToPlayer;
    private Vector2 lockedJumpDir;

    private bool inChaseRange  = false;
    private bool inAttackRange = false;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
        bodyCollider = ResolveBodyCollider();
        if (bodyCollider != null)
            originalExcludeLayers = bodyCollider.excludeLayers;
    }

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance.player != null)
        {
            playerTransform = GameManager.instance.player.transform;
            playerCollider  = GameManager.instance.player.GetComponent<Collider2D>();
        }
    }

    // Subscribe on enable / unsubscribe on disable so pooled instances re-wire cleanly on reuse.
    private void OnEnable()
    {
        entity.aiInteractionManager.TriggerChase            += OnEnterChaseRange;
        entity.aiInteractionManager.TriggerLeaveRange       += OnLeaveChaseRange;
        entity.aiInteractionManager.TriggerEnterAttackRange += OnEnterAttackRange;
        entity.aiInteractionManager.TriggerExitAttackRange  += OnExitAttackRange;
        entity.aiInteractionManager.TriggerAttached         += OnAttached;
        entity.aiInteractionManager.TriggerEclipse          += OnEclipseHit;
    }

    private void OnDisable()
    {
        entity.aiInteractionManager.TriggerChase            -= OnEnterChaseRange;
        entity.aiInteractionManager.TriggerLeaveRange       -= OnLeaveChaseRange;
        entity.aiInteractionManager.TriggerEnterAttackRange -= OnEnterAttackRange;
        entity.aiInteractionManager.TriggerExitAttackRange  -= OnExitAttackRange;
        entity.aiInteractionManager.TriggerAttached         -= OnAttached;
        entity.aiInteractionManager.TriggerEclipse          -= OnEclipseHit;
    }

    /// <summary>Resets the FSM, timers, range flags and physics body for a pooled respawn.</summary>
    public void ResetForSpawn()
    {
        state = MoveState.Idle;
        timer = 0f;
        inChaseRange  = false;
        inAttackRange = false;

        RestoreHitbox();                                 // undo any mid-jump exclude layers
        if (bodyCollider != null) bodyCollider.isTrigger = false;  // undo attach-time trigger flip

        entity.rb.bodyType        = RigidbodyType2D.Dynamic;       // undo attach-time Kinematic flip
        entity.rb.sleepMode       = RigidbodySleepMode2D.StartAwake;
        entity.rb.linearVelocity  = Vector2.zero;
        entity.rb.angularVelocity = 0f;
    }

    // ── Range events ─────────────────────────────────────────────────────────

    private void OnEnterChaseRange()
    {
        inChaseRange = true;
        if (state == MoveState.Idle) EnterChasing();
    }

    private void OnLeaveChaseRange()
    {
        inChaseRange = false;
        // A jump in progress is never interrupted — it completes, then the recovery
        // decision sees inChaseRange == false and returns the AI to Idle.
        if (state == MoveState.Chasing || state == MoveState.Idle) GoIdle();
    }

    private void OnEnterAttackRange() => inAttackRange = true;
    private void OnExitAttackRange()  => inAttackRange = false;

    // ── FSM transitions ────────────────────────────────────────────────────────

    private void EnterChasing()
    {
        state = MoveState.Chasing;
        entity.aiAnimatorManager.StartChase();
        currentSpeed = entity.aiStatsManager.GetMaxSpeed();
    }

    private void GoIdle()
    {
        state = MoveState.Idle;
        entity.aiAnimatorManager.StopChase();
        currentSpeed = 0f;
        entity.rb.linearVelocity = Vector2.zero;
    }

    private void EnterWindUp()
    {
        state = MoveState.WindUp;
        timer = 0f;
        entity.rb.linearVelocity = Vector2.zero;
        entity.aiAnimatorManager.PlayWindUp();
    }

    private void EnterJump()
    {
        // Launch-time aim: re-capture the direction so the lunge tracks the player's latest position.
        lockedJumpDir = ((Vector2)(playerTransform.position - transform.position)).normalized;
        if (bodyCollider != null)
            bodyCollider.excludeLayers = entity.aiStatsManager.GetJumpExcludeLayers();
        state = MoveState.Jumping;
        timer = 0f;
        entity.aiAnimatorManager.PlayJump();
    }

    private void Land()
    {
        RestoreHitbox();
        entity.rb.linearVelocity = Vector2.zero;
        state = MoveState.Recovery;
        timer = 0f;
        entity.aiAnimatorManager.JumpMissed();
    }

    private void DecideAfterRecovery()
    {
        if (!inChaseRange)        GoIdle();
        else if (inAttackRange)   EnterWindUp();
        else                      EnterChasing();
    }

    private void RestoreHitbox()
    {
        if (bodyCollider != null)
            bodyCollider.excludeLayers = originalExcludeLayers;
    }

    // ── Tick: decisions + timers (per frame) ───────────────────────────────────

    protected override void HandleDirectionalChange()
    {
        switch (state)
        {
            case MoveState.Chasing:
                directionToPlayer = ((Vector2)(playerTransform.position - transform.position)).normalized;
                if (inAttackRange) EnterWindUp();
                break;

            case MoveState.WindUp:
                directionToPlayer = ((Vector2)(playerTransform.position - transform.position)).normalized;
                timer += Time.deltaTime;
                if (timer >= entity.aiStatsManager.GetWindUpDuration()) EnterJump();
                break;

            case MoveState.Jumping:
                timer += Time.deltaTime;
                if (timer >= entity.aiStatsManager.GetJumpDuration()) Land();
                break;

            case MoveState.Recovery:
                timer += Time.deltaTime;
                if (timer >= entity.aiStatsManager.GetRecoveryDuration()) DecideAfterRecovery();
                break;
        }
    }

    // ── Tick: physics (fixed step) ─────────────────────────────────────────────

    protected override void HandleMovement()
    {
        switch (state)
        {
            case MoveState.Chasing:
                entity.rb.linearVelocity = directionToPlayer * currentSpeed;
                if(Mathf.Sign(directionToPlayer.x) == -1)
                {
                    Debug.Log("flip False");
                    entity.aiAnimatorManager.Flip(false);
                }
                else
                {
                    Debug.Log("flip True");
                    entity.aiAnimatorManager.Flip(true);
                }
                break;

            case MoveState.WindUp:
                entity.rb.linearVelocity = Vector2.zero;
                break;

            case MoveState.Jumping:
                // Direction stays locked for the whole jump — no rotation or re-aim.
                entity.rb.linearVelocity = lockedJumpDir *
                    (entity.aiStatsManager.GetJumpDistance() / entity.aiStatsManager.GetJumpDuration());
                break;

            case MoveState.Recovery:
            case MoveState.Dying:
                entity.rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    protected override void HandleCharacterPhysics() { }

    // ── Attach ─────────────────────────────────────────────────────────────────

    private void OnAttached()
    {
        if (playerTransform == null) return;

        // Direction the AI came in from — it ends up protruding from this side of the player.
        Vector2 dir = ((Vector2)(transform.position - playerTransform.position)).normalized;

        // Snap to the player's surface so roughly half the body sits inside the player.
        if (playerCollider != null)
            transform.position = playerCollider.ClosestPoint(transform.position);

        // Face outward, then ride the player for free (no per-frame follow code).
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.SetParent(playerTransform, true);

        RestoreHitbox();
        entity.rb.linearVelocity  = Vector2.zero;
        entity.rb.angularVelocity = 0f;

        // Stay in the simulation so the Eclipse (and the future player kill) can still detect it.
        // Kinematic → rides the player's transform instead of being driven by dynamics.
        // Trigger    → embedded in the player without physically shoving it, while still
        //              firing OnTriggerEnter2D against the Eclipse.
        entity.rb.bodyType = RigidbodyType2D.Kinematic;
        entity.rb.sleepMode = RigidbodySleepMode2D.NeverSleep;  // keep overlap tests alive while carried
        if (bodyCollider != null) bodyCollider.isTrigger = true;

        state = MoveState.Attached;
    }

    // ── Eclipse ──────────────────────────────────────────────────────────────────

    private void OnEclipseHit()
    {
        if (state == MoveState.Dying) return;

        // Halt wherever contact happened — for a jump that's the Eclipse border. Entering
        // Dying stops HandleMovement from re-applying the lunge velocity, so it parks at the
        // border (important for the death animation, to be added later) instead of flying on.
        RestoreHitbox();
        entity.rb.linearVelocity = Vector2.zero;
        state = MoveState.Dying;
        entity.aiAnimatorManager.PlayLand();
        entity.Die();
    }

    // The "hitbox" is the solid (non-trigger) collider; detection radii are triggers on the player.
    private Collider2D ResolveBodyCollider()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders)
            if (!c.isTrigger) return c;
        return colliders.Length > 0 ? colliders[0] : null;
    }
}
