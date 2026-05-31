using UnityEngine;


public class PlayerLocomotionManager : LocomotionManager
{
    private PlayerCharacterManager playerCharacter;
    private PlayerStatsManager statsManager;
    private PlayerInventoryManager inventoryManager;
    private InputManager inputManager;

    private float verticalMovementInput;
    private float horizontalMovementInput;

    protected override void Awake()
    {
        base.Awake();
        playerCharacter = GetComponent<PlayerCharacterManager>();
        statsManager = GetComponent<PlayerStatsManager>();
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
            verticalMovementInput = 0f;
            horizontalMovementInput = 0f;
            isSprinting = false;
        }
        else if (!canRotate)
        {
            horizontalMovementInput = 0f;
            isSprinting = inputManager.IsBoosting;
        }
        else
        {
            verticalMovementInput = inputManager.verticalInput;
            horizontalMovementInput = inputManager.horizontalInput;
            isSprinting = inputManager.IsBoosting;
        }
    }

    protected override void HandleMovement()
    {
        if (!canMove) return;

        float maxSpd = statsManager.GetMaxSpeed();
        float minSpd = statsManager.GetMinSpeed();

        // Vertical input: accelerate forward, brake/reverse, or auto-decelerate
        if (verticalMovementInput > 0f)
        {
            float accel = statsManager.GetAcceleration();
            if (isSprinting) accel *= statsManager.GetSprintSpeedMultiplier();
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpd, accel * Time.deltaTime);
        }
        else if (verticalMovementInput < 0f)
        {
            float manualDecel = statsManager.GetManualDeceleration();
            currentSpeed = Mathf.MoveTowards(currentSpeed, minSpd, manualDecel * Time.deltaTime);
        }
        else
        {
            float autoDecel = statsManager.GetAutoDeceleration();
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, autoDecel * Time.deltaTime);
        }

        // Horizontal input: binary full-left or full-right turn, scaled by speed ratio
        if (Mathf.Abs(currentSpeed) > 0.01f && horizontalMovementInput != 0f)
        {
            float turnDir = Mathf.Sign(horizontalMovementInput);
            float speedRatio = Mathf.Abs(currentSpeed) / maxSpd;
            float turnSpeed = statsManager.GetTurnSpeed();
            float rotation = turnDir * turnSpeed * speedRatio * Time.deltaTime;

            // Flip turn direction when reversing so steering feels natural
            if (currentSpeed < 0f) rotation = -rotation;

            transform.Rotate(0f, 0f, -rotation);
        }

        // Drift: lerp actual velocity toward heading direction using tire friction.
        // Lower tireFriction = more drift (dirt terrain feel).
        Vector2 targetVelocity = transform.up * currentSpeed;
        playerCharacter.rb.linearVelocity = Vector2.Lerp(
            playerCharacter.rb.linearVelocity,
            targetVelocity,
            statsManager.GetTireFriction() * Time.deltaTime
        );
    }

    protected override void HandleCharacterPhysics()
    {
        // Additional physics handling can be implemented here if needed
    }

    private void HandleFuelConsumption()
    {
        if (isSprinting && Mathf.Abs(currentSpeed) > 0.01f)
        {
            float fuelConsumed = statsManager.GetFuelConsumptionRate() * statsManager.GetBoostingFuelConsumptionMultiplier() * Time.deltaTime;
            inventoryManager.ConsumeFuel(fuelConsumed);
        }
        else if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            float fuelConsumed = statsManager.GetFuelConsumptionRate() * Time.deltaTime;
            inventoryManager.ConsumeFuel(fuelConsumed);
        }
    }
}