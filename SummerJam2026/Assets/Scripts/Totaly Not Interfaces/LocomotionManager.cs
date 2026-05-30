using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    public CharacterManager character;
    public StatsManager stats;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canRotate = true;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public float currentSpeed = 0f;

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
        if (!canMove) return;

        HandleMovement();
        HandleCharacterPhysics();
    }

    protected virtual void HandleCharacterPhysics()
    {
        // Implement physics handling logic here
    }

    protected virtual void HandleMovement()
    {
        // Implement movement logic here
    }
}
