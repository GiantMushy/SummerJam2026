using UnityEngine;


public class PlayerLocomotionManager : LocomotionManager
{
    private PlayerCharacterManager playerCharacter;
    private PlayerStatsManager statsManager;
    private PlayerInventoryManager inventoryManager;
    private PlayerInputManager inputManager;

    private float verticalMovementInput;
    private float horizontalMovementInput;
    private float currentSpeed;

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
        inputManager = PlayerInputManager.instance;
    }

    protected override void Update()
    {
        base.Update();
        HandleMovementInput();
        Move();
    }

    protected virtual void HandleMovementInput()
    {
        verticalMovementInput = inputManager.verticalInput;
        horizontalMovementInput = inputManager.horizontalInput;
    }

    protected virtual void Move()
    {
        
    }
}