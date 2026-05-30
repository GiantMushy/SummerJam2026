using UnityEngine;

public class AiCharacterEffectsManager : CharacterEffectsManager
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
}