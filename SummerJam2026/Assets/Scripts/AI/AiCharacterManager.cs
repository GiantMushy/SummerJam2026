using UnityEngine;

[RequireComponent(typeof(CharacterSoundEffectManager))]
[RequireComponent(typeof(AiCharacterEffectsManager))]
[RequireComponent(typeof(AiInteractionManager))]
[RequireComponent(typeof(AiLocomotionManager))]
[RequireComponent(typeof(AiInventoryManager))]
[RequireComponent(typeof(AiEquipmentManager))]
[RequireComponent(typeof(AiAnimatorManager))]
[RequireComponent(typeof(AiCombatManager))]
[RequireComponent(typeof(AiStatsManager))]
[RequireComponent(typeof(AiUiManager))]
public class AiCharacterManager : CharacterManager
{
    [HideInInspector] public AiCharacterEffectsManager aiCharacterEffectsManager;
    [HideInInspector] public CharacterSoundEffectManager aiSoundEffectsManager;
    [HideInInspector] public AiInteractionManager aiInteractionManager;
    [HideInInspector] public AiLocomotionManager aiLocomotionManager;
    [HideInInspector] public AiInventoryManager aiInventoryManager;
    [HideInInspector] public AiEquipmentManager aiEquipmentManager;
    [HideInInspector] public AiAnimatorManager aiAnimatorManager;
    [HideInInspector] public AiCombatManager aiCombatManager;
    [HideInInspector] public AiStatsManager aiStatsManager;
    [HideInInspector] public AiUiManager aiUiManager;

    protected override void Awake()
    {
        base.Awake();
        aiSoundEffectsManager =     GetComponent<CharacterSoundEffectManager>();
        aiCharacterEffectsManager = GetComponent<AiCharacterEffectsManager>();
        aiInteractionManager =      GetComponent<AiInteractionManager>();
        aiLocomotionManager =       GetComponent<AiLocomotionManager>();
        aiInventoryManager =        GetComponent<AiInventoryManager>();
        aiEquipmentManager =        GetComponent<AiEquipmentManager>();
        aiAnimatorManager =         GetComponent<AiAnimatorManager>();
        aiCombatManager =           GetComponent<AiCombatManager>();
        aiStatsManager =            GetComponent<AiStatsManager>();
        aiUiManager =               GetComponent<AiUiManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Die()
    {
        aiStatsManager.ActivateDyingState();
        Destroy(gameObject);
    }
}