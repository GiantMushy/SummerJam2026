using UnityEngine;

public class AiCharacterManager : CharacterManager
{
    private AiCharacterEffectsManager characterEffectsManager;
    private AiLocomotionManager locomotionManager;
    private AiInventoryManager inventoryManager;
    private SoundEffectsManager soundEffectsManager;
    private AiEquipmentManager equipmentManager;
    private AiAnimatorManager animatorManager;
    private AiCombatManager combatManager;
    private AiStatsManager statsManager;
    private AiUiManager uiManager;

    protected override void Awake()
    {
        base.Awake();
        characterEffectsManager =   GetComponent<AiCharacterEffectsManager>();
        locomotionManager =         GetComponent<AiLocomotionManager>();
        inventoryManager =          GetComponent<AiInventoryManager>();
        soundEffectsManager =       GetComponent<SoundEffectsManager>();
        equipmentManager =          GetComponent<AiEquipmentManager>();
        animatorManager =           GetComponent<AiAnimatorManager>();
        statsManager =              GetComponent<AiStatsManager>();
        uiManager =                 GetComponent<AiUiManager>();
        combatManager =             GetComponent<AiCombatManager>();
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