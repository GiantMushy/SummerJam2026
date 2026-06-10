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

    // Register on enable / unregister on disable so pooled instances (activated via SetActive)
    // re-join and leave the tick cleanly without Destroy. AiManager exists before any AI is
    // pooled at runtime, so instance is always set by the time OnEnable runs.
    void OnEnable() => AiManager.instance.Register(this);

    void OnDisable() => AiManager.instance.Unregister(this);

    // Called once per frame by AiManager instead of per-instance Unity messages.
    public void TickUpdate()
    {
        aiLocomotionManager.TickUpdate();
        aiCombatManager.TickCombat();
    }

    public void TickFixedUpdate()
    {
        aiLocomotionManager.TickFixedUpdate();
    }

    [SerializeField] private float deathCleanupDelay = 1.5f;

    public override void Die()
    {
        transform.SetParent(null); // detach from car immediately so the body doesn't follow
        aiStatsManager.ActivateDyingState();
        aiAnimatorManager.Die();
        Invoke(nameof(Remove), deathCleanupDelay);
    }

    public void Remove() => AiPool.instance.Despawn(this);

    /// <summary>
    /// Returns every manager's mutable runtime state to a fresh-spawn baseline so a reused
    /// (pooled) instance behaves exactly like a newly instantiated one. Called by AiPool.Spawn
    /// after the object is reactivated.
    /// </summary>
    public void PrepareForSpawn()
    {
        CancelInvoke();                  // clear any pending Remove from a prior death
        transform.SetParent(null);
        transform.rotation = Quaternion.identity;

        aiStatsManager.ResetForSpawn();
        aiCombatManager.ResetForSpawn();
        aiLocomotionManager.ResetForSpawn();
        aiInteractionManager.ResetForSpawn();
        aiAnimatorManager.ResetForSpawn();
    }
}