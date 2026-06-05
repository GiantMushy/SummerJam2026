using UnityEngine;

public class AiAnimatorManager : AnimatorManager
{
    private AiCharacterManager entity;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    // Jump-attack animation hooks, called by AiLocomotionManager at FSM transitions.
    // Guarded so AI without an Animator assigned don't throw.
    private static readonly int WindUpHash = Animator.StringToHash("WindUp");
    private static readonly int JumpHash   = Animator.StringToHash("Jump");
    private static readonly int LandHash   = Animator.StringToHash("Land");

    public void PlayWindUp() { if (animator != null) animator.SetTrigger(WindUpHash); }
    public void PlayJump()   { if (animator != null) animator.SetTrigger(JumpHash); }
    public void PlayLand()   { if (animator != null) animator.SetTrigger(LandHash); }
}