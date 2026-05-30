using UnityEngine;

public class PlayerEquipmentManager : EquipmentManager
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