using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb; 
    [HideInInspector] public LocomotionManager locomotion;
    [HideInInspector] public EquipmentManager equipment;
    [HideInInspector] public InventoryManager inventory;
    [HideInInspector] public AnimatorManager animationManager;
    [HideInInspector] public CharacterSoundEffectManager soundEffects;
    [HideInInspector] public CharacterEffectsManager effects;     
    [HideInInspector] public CombatManager combat;
    [HideInInspector] public StatsManager stats;
    [HideInInspector] public InteractionManager interaction;
    [HideInInspector] public UiManager ui;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        locomotion = GetComponent<LocomotionManager>();
        equipment = GetComponent<EquipmentManager>();
        inventory = GetComponent<InventoryManager>();
        animationManager = GetComponent<AnimatorManager>();
        soundEffects = GetComponent<CharacterSoundEffectManager>();
        effects = GetComponent<CharacterEffectsManager>();
        combat = GetComponent<CombatManager>();
        stats = GetComponent<StatsManager>();
        interaction = GetComponent<InteractionManager>();
        ui = GetComponent<UiManager>();
    }

    protected virtual void Start()
    {
        
    }
    
}