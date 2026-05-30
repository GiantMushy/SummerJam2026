using UnityEngine;

public class PlayerCharacterManager : CharacterManager
{
    private PlayerCharacterEffectsManager characterEffectsManager;
    private PlayerLocomotionManager locomotionManager;
    private PlayerInventoryManager inventoryManager;
    private SoundEffectsManager soundEffectsManager;
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
        soundEffectsManager =       GetComponent<SoundEffectsManager>();
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

    protected override void Update()
    {
        base.Update();
    }
}