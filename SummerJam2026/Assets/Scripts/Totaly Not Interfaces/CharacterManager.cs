using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterLocomotionManager))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(AnimationManager))]
[RequireComponent(typeof(SoundEffectManager))]
[RequireComponent(typeof(CharacterEffectsManager))]
[RequireComponent(typeof(CombatManager))]
[RequireComponent(typeof(StatsManager))]
[RequireComponent(typeof(InteractionManager))]
[RequireComponent(typeof(UIManager))]   
public class CharacterManager : MonoBehaviour
{
    public Rigidbody2D rb; 
    public CharacterLocomotionManager locomotion;
    public EquipmentManager equipment;
    public InventoryManager inventory;
    public AnimationManager animation;
    public SoundEffectManager soundEffects;
    public CharacterEffectsManager effects;     
    public CombatManager combat;
    public StatsManager stats;
    public InteractionManager interaction;
    public UIManager ui;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        locomotion = GetComponent<CharacterLocomotionManager>();
        equipment = GetComponent<EquipmentManager>();
        inventory = GetComponent<InventoryManager>();
        animation = GetComponent<AnimationManager>();
        soundEffects = GetComponent<SoundEffectManager>();
        effects = GetComponent<CharacterEffectsManager>();
        combat = GetComponent<CombatManager>();
        stats = GetComponent<StatsManager>();
        interaction = GetComponent<InteractionManager>();
        ui = GetComponent<UIManager>();
    }
}