using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    [HideInInspector] public CharacterManager character;
    [HideInInspector] public StatsManager stats;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canRotate = true;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public float currentSpeed = 0f;
    protected float currentFriction = 1f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
        stats = GetComponent<StatsManager>();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        HandleDirectionalChange();
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
        HandleCharacterPhysics();
    }

    protected virtual void HandleDirectionalChange()
    {
        // Implement movement logic here
    }

    protected virtual void HandleMovement()
    {
        // Implement movement logic here
    }

    protected virtual void HandleCharacterPhysics()
    {
        // Implement physics handling logic here
    }
}
