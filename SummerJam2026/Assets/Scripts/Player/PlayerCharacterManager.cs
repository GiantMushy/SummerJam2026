using UnityEngine;

public class PlayerCharacterManager : CharacterManager
{
    public PlayerEffectsManager playerEffectsManager;
    public PlayerLocomotionManager playerLocomotionManager;
    public PlayerInventoryManager playerInventoryManager;
    public CharacterSoundEffectManager characterSoundEffectsManager;
    public PlayerEquipmentManager playerEquipmentManager;
    public PlayerAnimatorManager playerAnimatorManager;
    public PlayerCombatManager playerCombatManager;
    public PlayerStatsManager playerStatsManager;
    public PlayerUiManager uiManager;

    protected override void Awake()
    {
        base.Awake();
        characterSoundEffectsManager = GetComponent<CharacterSoundEffectManager>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerAnimatorManager =  GetComponent<PlayerAnimatorManager>();
        playerEffectsManager = GetComponent<PlayerEffectsManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        uiManager = GetComponent<PlayerUiManager>();
    }

    protected override void Start()
    {
        base.Start();
        InputManager.instance.player = this;
    }
}