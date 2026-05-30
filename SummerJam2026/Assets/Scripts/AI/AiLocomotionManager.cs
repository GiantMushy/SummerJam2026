using UnityEngine;


public class AiLocomotionManager : LocomotionManager
{
    private AiCharacterManager entity;
    private AiInputManager inputManager;

    private float verticalMovement;
    private float horizontalMovement;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        HandleMovement();
        Move();
    }

    protected virtual void HandleMovement()
    {
        
    }

    protected virtual void Move()
    {
        
    }
}