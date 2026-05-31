using UnityEngine;


public class PlayerLocomotionManager : LocomotionManager
{
    private PlayerCharacterManager playerCharacter;
    private PlayerStatsManager statsManager;
    private PlayerInventoryManager inventoryManager;
    private InputManager inputManager;

    private float verticalMovementInput;
    private float horizontalMovementInput;

    private bool  isDrifting    = false;
    private float turnHoldTimer = 0f;
    private float neutralTimer  = 0f;
    private bool  isInNeutral   = false;

    protected override void Awake()
    {
        base.Awake();
        playerCharacter = GetComponent<PlayerCharacterManager>();
        statsManager    = GetComponent<PlayerStatsManager>();
        inventoryManager = GetComponent<PlayerInventoryManager>();
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
        // AI Garbage, feels horrible, will be fixed later
        if (!canMove)
        {
            isDrifting    = false;
            currentFriction = 1f;
            turnHoldTimer = 0f;
            neutralTimer  = 0f;
            isInNeutral   = false;
            return;
        }

        float maxSpd = statsManager.GetMaxSpeed();
        float dt     = Time.deltaTime;

        // Neutral gear: pause at zero before reverse engages
        if (isInNeutral)
        {
            neutralTimer += dt;
            currentSpeed  = Mathf.MoveTowards(currentSpeed, 0f, statsManager.GetAutoDeceleration() * dt);
            if (neutralTimer >= statsManager.GetReverseNeutralDelay())
                isInNeutral = false;

            playerCharacter.rb.linearVelocity = Vector2.Lerp(
                playerCharacter.rb.linearVelocity,
                transform.up * currentSpeed,
                currentFriction
            );
            return;
        }

        // Acceleration / braking
        if (verticalMovementInput > 0f)
        {
            float accel    = statsManager.GetAcceleration();
            if (isSprinting) accel *= statsManager.GetSprintSpeedMultiplier();
            float speedCap = isDrifting ? statsManager.GetDriftingMaxSpeed() : maxSpd;
            currentSpeed   = Mathf.MoveTowards(currentSpeed, speedCap, accel * dt);
        }
        else if (verticalMovementInput < 0f)
        {
            float prevSpeed = currentSpeed;
            currentSpeed    = Mathf.MoveTowards(currentSpeed, statsManager.GetMinSpeed(), statsManager.GetManualDeceleration() * dt);

            // Crossed zero: enter neutral instead of immediately reversing
            if (prevSpeed > 0f && currentSpeed <= 0f)
            {
                currentSpeed = 0f;
                isInNeutral  = true;
                neutralTimer = 0f;
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, statsManager.GetAutoDeceleration() * dt);
        }

        // Steering: binary direction, scaled by speed ratio, flipped in reverse
        if (Mathf.Abs(currentSpeed) > 0.01f && horizontalMovementInput != 0f)
        {
            float speedRatio = Mathf.Abs(currentSpeed) / maxSpd;
            float rotation   = Mathf.Sign(horizontalMovementInput) * statsManager.GetTurnSpeed() * speedRatio * dt;
            if (currentSpeed < 0f) rotation = -rotation;
            transform.Rotate(0f, 0f, -rotation);
        }

        // Drift activation: must be above speed threshold and hold a turn long enough
        float driftThreshold = maxSpd * statsManager.GetDriftSpeedThreshold();
        if (Mathf.Abs(currentSpeed) >= driftThreshold && Mathf.Abs(horizontalMovementInput) > 0f)
        {
            turnHoldTimer += dt;
            if (turnHoldTimer >= statsManager.GetDriftEntryTime())
                isDrifting = true;
        }
        else
        {
            turnHoldTimer = 0f;
        }

        // Drift deactivation: speed dropped back below threshold
        if (isDrifting && Mathf.Abs(currentSpeed) < driftThreshold)
            isDrifting = false;

        // Friction: lerps to minFriction while drifting+turning, recovers to full grip otherwise
        float targetFriction = (isDrifting && Mathf.Abs(horizontalMovementInput) > 0f)
            ? statsManager.GetMinFriction()
            : 1f;
        float frictionRate = targetFriction < currentFriction
            ? statsManager.GetFrictionDecayRate()
            : statsManager.GetFrictionRecoveryRate();
        currentFriction = Mathf.MoveTowards(currentFriction, targetFriction, frictionRate * dt);

        playerCharacter.rb.linearVelocity = Vector2.Lerp(
            playerCharacter.rb.linearVelocity,
            transform.up * currentSpeed,
            currentFriction
        );
    }

    protected override void HandleCharacterPhysics()
    {
        // Additional physics handling can be implemented here if needed
    }

    private void HandleFuelConsumption()
    {
        if (Mathf.Abs(currentSpeed) <= 0.01f) return;

        float rate = statsManager.GetFuelConsumptionRate();
        if (isSprinting) rate *= statsManager.GetBoostingFuelConsumptionMultiplier();
        inventoryManager.ConsumeFuel(rate * Time.deltaTime);
    }
}
