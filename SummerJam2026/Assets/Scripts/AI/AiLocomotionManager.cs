using UnityEngine;


public class AiLocomotionManager : LocomotionManager
{
    private AiCharacterManager entity;

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
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void HandleDirectionalChange()
    {
        
    }

    protected override void HandleMovement()
    {
        
    }

    protected override void HandleCharacterPhysics()
    {
        
    }
}