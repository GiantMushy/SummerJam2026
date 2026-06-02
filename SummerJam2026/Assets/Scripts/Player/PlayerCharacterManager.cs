using UnityEngine;

public class PlayerCharacterManager : CharacterManager
{
    [HideInInspector] public PlayerEffectsManager playerEffectsManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public CharacterSoundEffectManager characterSoundEffectsManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerUiManager uiManager;

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