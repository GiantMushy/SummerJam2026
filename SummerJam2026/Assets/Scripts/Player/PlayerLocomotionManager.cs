using UnityEngine;

public class PlayerLocomotionManager : LocomotionManager
{
    private PlayerCharacterManager playerCharacter;
    private PlayerStatsManager     statsManager;
    private PlayerInventoryManager inventoryManager;
    private InputManager           inputManager;
    private Rigidbody2D            rb;

    private float verticalMovementInput;
    private float horizontalMovementInput;

    private bool  isDrifting       = false;
    private bool  isRecoveringGrip = false;
    private float neutralTimer     = 0f;
    private bool  isInNeutral      = false;
    private float currentGrip      = 1f;

    protected override void Awake()
    {
        base.Awake();
        playerCharacter  = GetComponent<PlayerCharacterManager>();
        statsManager     = GetComponent<PlayerStatsManager>();
        inventoryManager = GetComponent<PlayerInventoryManager>();
        rb               = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        inputManager = InputManager.instance;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //HandleFuelConsumption();
    }

    protected override void HandleDirectionalChange()
    {
        if (!canMove)
        {
            verticalMovementInput   = 0f;
            horizontalMovementInput = 0f;
            isSprinting             = false;
        }
        else if (!canRotate)
        {
            horizontalMovementInput = 0f;
            isSprinting             = inputManager.IsBoosting;
        }
        else
        {
            verticalMovementInput   = inputManager.verticalInput;
            horizontalMovementInput = inputManager.horizontalInput;
            isSprinting             = inputManager.IsBoosting;
        }
    }

    protected override void HandleMovement()
    {
        if (!canMove)
        {
            isDrifting       = false;
            isRecoveringGrip = false;
            currentGrip      = 1f;
            neutralTimer     = 0f;
            isInNeutral      = false;
            return;
        }

        currentSpeed = Vector2.Dot(rb.linearVelocity, transform.up);

        HandleDriftState();
        HandleAcceleration();
        HandleSteering();
        HandleGrip();
    }

    protected override void HandleCharacterPhysics() { }

    // ── Acceleration / Gears ─────────────────────────────────────────────────

    private void HandleAcceleration()
    {
        float maxSpd = stats.GetMaxSpeed() * (isSprinting ? stats.GetSprintSpeedMultiplier() : 1f);
        float minSpd = stats.GetMinSpeed();

        bool pressingForward = verticalMovementInput >  0.1f;
        bool pressingBack    = verticalMovementInput < -0.1f;
        bool movingForward   = currentSpeed >  0.05f;

        if (pressingForward)
        {
            if (currentSpeed >= -0.05f) // Stopped or already going forward
            {
                neutralTimer = 0f;
                if (currentSpeed < maxSpd)
                    rb.AddForce((Vector2)transform.up * GetGearAcceleration());
            }
            else // Going backward — wait out neutral delay before going forward
            {
                neutralTimer += Time.fixedDeltaTime;
                if (neutralTimer >= statsManager.GetReverseNeutralDelay() && currentSpeed < maxSpd)
                    rb.AddForce((Vector2)transform.up * GetGearAcceleration());
            }
        }
        else if (pressingBack)
        {
            if (movingForward && !isDrifting)
            {
                neutralTimer = 0f;
                rb.AddForce(-(Vector2)transform.up * stats.GetManualDeceleration());
            }
            else if (!movingForward) // Stopped or reversing — wait out neutral delay before reversing
            {
                neutralTimer += Time.fixedDeltaTime;
                if (neutralTimer >= statsManager.GetReverseNeutralDelay() && currentSpeed > minSpd)
                    rb.AddForce((Vector2)transform.up * verticalMovementInput * GetGearAcceleration() * 0.5f);
            }
        }
        else // No input — coast to a stop
        {
            neutralTimer = 0f;
            if (!isDrifting && Mathf.Abs(currentSpeed) > 0.05f)
                rb.AddForce(-(Vector2)transform.up * Mathf.Sign(currentSpeed) * stats.GetAutoDeceleration());
        }

        ClampForwardSpeed(minSpd, maxSpd);
    }

    private float GetGearAcceleration()
    {
        float spd = Mathf.Abs(currentSpeed);
        if (spd < statsManager.GetGear1SpeedThreshold()) return statsManager.GetGear1Acceleration();
        if (spd < statsManager.GetGear2SpeedThreshold()) return statsManager.GetGear2Acceleration();
        return statsManager.GetGear3Acceleration();
    }

    private void ClampForwardSpeed(float minSpd, float maxSpd)
    {
        float fwdSpd = Vector2.Dot(rb.linearVelocity, transform.up);
        if      (fwdSpd > maxSpd) rb.linearVelocity -= (Vector2)transform.up * (fwdSpd - maxSpd);
        else if (fwdSpd < minSpd) rb.linearVelocity -= (Vector2)transform.up * (fwdSpd - minSpd);
    }

    // ── Steering ─────────────────────────────────────────────────────────────

    private void HandleSteering()
    {
        if (Mathf.Abs(horizontalMovementInput) < 0.01f) return;

        float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / statsManager.GetOptimalTurnSpeed());
        float turnRate    = statsManager.GetTurnSpeed() * Mathf.SmoothStep(0f, 1f, speedFactor);
        if (isDrifting) turnRate *= statsManager.GetDriftTurnMultiplier();

        // Negative makes D (horizontal +1) rotate clockwise = turn right for an upward-facing sprite
        // If turning feels reversed, flip the sign here
        float turnDelta = -horizontalMovementInput * turnRate * Time.fixedDeltaTime;
        if (currentSpeed < 0f) turnDelta = -turnDelta; // Invert when reversing

        rb.MoveRotation(rb.rotation + turnDelta);
    }

    // ── Grip / Lateral Velocity Cancellation ─────────────────────────────────

    private void HandleGrip()
    {
        Vector2 lateralVel = Vector2.Dot(rb.linearVelocity, transform.right) * (Vector2)transform.right;
        rb.linearVelocity -= lateralVel * currentGrip;
    }

    // ── Drift State Machine ───────────────────────────────────────────────────

    private void HandleDriftState()
    {
        bool pressingBack   = verticalMovementInput < -0.1f;
        bool isTurning      = Mathf.Abs(horizontalMovementInput) > 0.1f;
        bool aboveThreshold = currentSpeed > statsManager.GetDriftSpeedThreshold();

        if (!isDrifting && !isRecoveringGrip)
        {
            if (pressingBack && isTurning && aboveThreshold)
                EnterDrift();
        }
        else if (isDrifting)
        {
            if (!pressingBack || currentSpeed < 0.5f)
            {
                ExitDrift();
            }
            else
            {
                currentGrip = statsManager.GetMinFriction();

                // Bleed speed passively during drift so it doesn't feel floaty
                rb.AddForce(-(Vector2)transform.up * stats.GetAutoDeceleration() * 0.3f);

                float fwdSpd = Vector2.Dot(rb.linearVelocity, transform.up);
                if (fwdSpd > statsManager.GetDriftingMaxSpeed())
                    rb.linearVelocity -= (Vector2)transform.up * (fwdSpd - statsManager.GetDriftingMaxSpeed());
            }
        }
        else if (isRecoveringGrip)
        {
            currentGrip += statsManager.GetFrictionRecoveryRate() * Time.fixedDeltaTime;
            if (currentGrip >= 0.99f)
            {
                currentGrip      = 1f;
                isRecoveringGrip = false;
            }
        }
    }

    private void EnterDrift()
    {
        isDrifting  = true;
        currentGrip = statsManager.GetMinFriction();
        // Negative sign: D (horizontal +1) → clockwise angular kick → rear swings left, nose swings right
        // If kick feels reversed, flip the sign here
        rb.angularVelocity += -statsManager.GetDriftAngularImpulse() * horizontalMovementInput;
    }

    private void ExitDrift()
    {
        isDrifting       = false;
        isRecoveringGrip = true;
    }

    private void HandleFuelConsumption()
    {
        if (Mathf.Abs(currentSpeed) <= 0.01f) return;
        float rate = statsManager.GetFuelConsumptionRate();
        if (isSprinting) rate *= statsManager.GetBoostingFuelConsumptionMultiplier();
        inventoryManager.ConsumeFuel(rate * Time.deltaTime);
    }
}
