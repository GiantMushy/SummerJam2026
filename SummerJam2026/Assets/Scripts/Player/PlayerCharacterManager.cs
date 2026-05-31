using UnityEngine;

[RequireComponent(typeof(PlayerLocomotionManager))]
[RequireComponent(typeof(PlayerEquipmentManager))]
[RequireComponent(typeof(PlayerInventoryManager))]
[RequireComponent(typeof(PlayerAnimatorManager))]
[RequireComponent(typeof(SoundEffectManager))]
[RequireComponent(typeof(PlayerCharacterEffectsManager))]
[RequireComponent(typeof(PlayerCombatManager))]
[RequireComponent(typeof(PlayerStatsManager))]
[RequireComponent(typeof(PlayerInteractionManager))]
[RequireComponent(typeof(PlayerUiManager))]
public class PlayerCharacterManager : CharacterManager
{
    private PlayerCharacterEffectsManager characterEffectsManager;
    private PlayerLocomotionManager locomotionManager;
    private PlayerInventoryManager inventoryManager;
    private SoundEffectManager soundEffectsManager;
    private PlayerEquipmentManager equipmentManager;
    private PlayerAnimatorManager animatorManager;
    private PlayerCombatManager combatManager;
    private PlayerStatsManager statsManager;
    private PlayerUiManager uiManager;

    protected override void Awake()
    {
        base.Awake();
        characterEffectsManager =   GetComponent<PlayerCharacterEffectsManager>();
        locomotionManager =         GetComponent<PlayerLocomotionManager>();
        inventoryManager =          GetComponent<PlayerInventoryManager>();
        soundEffectsManager =       GetComponent<SoundEffectManager>();
        equipmentManager =          GetComponent<PlayerEquipmentManager>();
        animatorManager =           GetComponent<PlayerAnimatorManager>();
        statsManager =              GetComponent<PlayerStatsManager>();
        uiManager =                 GetComponent<PlayerUiManager>();
        combatManager =             GetComponent<PlayerCombatManager>();
    }

    protected override void Start()
    {
        base.Start();
        InputManager.instance.player = this;
    }
}