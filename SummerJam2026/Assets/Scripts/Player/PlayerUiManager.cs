using UnityEngine;

public class PlayerUiManager : UiManager
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