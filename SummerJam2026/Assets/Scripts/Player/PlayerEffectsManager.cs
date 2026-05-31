using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    private PlayerCharacterManager player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
    }
}