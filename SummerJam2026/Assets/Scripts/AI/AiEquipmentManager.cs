using UnityEngine;

public class AiEquipmentManager : EquipmentManager
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