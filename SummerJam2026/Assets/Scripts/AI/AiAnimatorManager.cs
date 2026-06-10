using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AiAnimatorManager : AnimatorManager
{
    private AiCharacterManager entity;
    private SpriteRenderer sprite;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
        sprite = GetComponent<SpriteRenderer>();
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

    private static readonly int ChaseHash = Animator.StringToHash("Chasing");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int MissedHash = Animator.StringToHash("Missed");

    public void PlayWindUp() { animator.SetTrigger(WindUpHash); }
    public void StartChase(){ animator.SetBool(ChaseHash,true);}
    public void StopChase(){ animator.SetBool(ChaseHash,false);}
    public void JumpMissed(){ animator.SetTrigger(MissedHash);}
    public void Die(){ animator.SetTrigger(DeathHash); }
    public void PlayJump()
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.nameHash == JumpHash) { animator.SetTrigger(JumpHash); return; }
    }
    public void PlayLand()   { animator.SetTrigger(LandHash); }
    public void Flip(bool flip){
        if(sprite == null) return;
        sprite.flipX = flip;
    }
}