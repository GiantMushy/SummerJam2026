using UnityEngine;

public class PlayerLocomotionManager : LocomotionManager
{
    private PlayerCharacterManager player;

    private float verticalMovementInput;
    private float horizontalMovementInput;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()      => TickUpdate();
    void FixedUpdate() => TickFixedUpdate();

    public override void TickUpdate()
    {
        base.TickUpdate();
        HandleFuelConsumption();
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
        }
        else
        {
            verticalMovementInput   = InputManager.instance.verticalInput;
            horizontalMovementInput = InputManager.instance.horizontalInput;
        }
    }

    protected override void HandleMovement()
    {
        if (!canMove) return;

        currentSpeed = Vector2.Dot(player.rb.linearVelocity, transform.up);

        HandleSteering();
        HandleAcceleration();
        HandleGrip();
    }

    protected override void HandleCharacterPhysics() { }

    // ── Steering ─────────────────────────────────────────────────────────────

    private void HandleSteering()
    {
        if (Mathf.Abs(horizontalMovementInput) < 0.01f) return;

        float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / player.playerStatsManager.GetOptimalTurnSpeed());
        float turnRate    = player.playerStatsManager.GetTurnSpeed() * Mathf.SmoothStep(0f, 1f, speedFactor);
        float turnDelta   = -horizontalMovementInput * turnRate * Time.fixedDeltaTime;
        if (currentSpeed < 0f) turnDelta = -turnDelta;

        player.rb.MoveRotation(player.rb.rotation + turnDelta);
    }

    // ── Acceleration ─────────────────────────────────────────────────────────

    private void HandleAcceleration()
    {
        float maxSpd = stats.GetMaxSpeed() * (isSprinting ? stats.GetSprintSpeedMultiplier() : 1f);
        float minSpd = stats.GetMinSpeed();

        if (player.playerInventoryManager.GetFuel() <= 0f)
        {
            HandleStall(minSpd, maxSpd);
            return;
        }

        bool pressingForward = verticalMovementInput >  0.1f;
        bool pressingBack    = verticalMovementInput < -0.1f;
        bool movingForward   = currentSpeed >  0.05f;

        if (pressingForward)
        {
            if (currentSpeed < maxSpd)
                player.rb.AddForce((Vector2)transform.up * GetGearAcceleration());
        }
        else if (pressingBack)
        {
            if (movingForward)
                player.rb.AddForce(-(Vector2)transform.up * stats.GetManualDeceleration());
            else if (currentSpeed > minSpd)
                player.rb.AddForce((Vector2)transform.up * verticalMovementInput * GetGearAcceleration() * 0.5f);
        }
        else
        {
            if (Mathf.Abs(currentSpeed) > 0.05f)
                player.rb.AddForce(-(Vector2)transform.up * Mathf.Sign(currentSpeed) * stats.GetAutoDeceleration());
        }

        ClampForwardSpeed(minSpd, maxSpd);
    }

    private void HandleStall(float minSpd, float maxSpd)
    {
        bool pressingBack  = verticalMovementInput < -0.1f;
        bool movingForward = currentSpeed > 0.05f;

        if (pressingBack && movingForward)
            player.rb.AddForce(-(Vector2)transform.up * stats.GetManualDeceleration());
        else if (Mathf.Abs(currentSpeed) > 0.05f)
            player.rb.AddForce(-(Vector2)transform.up * Mathf.Sign(currentSpeed) * stats.GetAutoDeceleration());

        ClampForwardSpeed(minSpd, maxSpd);
    }

    private float GetGearAcceleration()
    {
        float spd = Mathf.Abs(currentSpeed);
        if (spd < player.playerStatsManager.GetGear1SpeedThreshold()) return player.playerStatsManager.GetGear1Acceleration();
        if (spd < player.playerStatsManager.GetGear2SpeedThreshold()) return player.playerStatsManager.GetGear2Acceleration();
        return player.playerStatsManager.GetGear3Acceleration();
    }

    private void ClampForwardSpeed(float minSpd, float maxSpd)
    {
        float fwdSpd = Vector2.Dot(player.rb.linearVelocity, transform.up);
        if      (fwdSpd > maxSpd) player.rb.linearVelocity -= (Vector2)transform.up * (fwdSpd - maxSpd);
        else if (fwdSpd < minSpd) player.rb.linearVelocity -= (Vector2)transform.up * (fwdSpd - minSpd);
    }

    // ── Grip ─────────────────────────────────────────────────────────────────

    private void HandleGrip()
    {
        // Cancel all lateral velocity so the car always moves in its snapped facing direction.
        Vector2 lateralVel = Vector2.Dot(player.rb.linearVelocity, transform.right) * (Vector2)transform.right;
        player.rb.linearVelocity -= lateralVel;
    }

    // ── Fuel ─────────────────────────────────────────────────────────────────

    private void HandleFuelConsumption()
    {
        bool pressingForward = verticalMovementInput >  0.1f;
        bool pressingBack    = verticalMovementInput < -0.1f;
        bool movingForward   = currentSpeed >  0.05f;
        bool movingBackward  = currentSpeed < -0.05f;

        bool braking    = (pressingForward && movingBackward) || (pressingBack && movingForward);
        bool throttling = (pressingForward || pressingBack) && !braking;
        if (!throttling) return;

        float rate = player.playerStatsManager.GetFuelConsumptionRate();
        if (isSprinting) rate *= player.playerStatsManager.GetBoostingFuelConsumptionMultiplier();
        player.playerInventoryManager.ConsumeFuel(rate * Time.deltaTime);
    }
}
