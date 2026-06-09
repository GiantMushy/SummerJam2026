using UnityEngine;

public class PlayerAnimatorManager : AnimatorManager
{
    private PlayerCharacterManager player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
        animatorGameObject = animator.gameObject;
    }

    protected override void Start()
    {
        base.Start();
    }
}