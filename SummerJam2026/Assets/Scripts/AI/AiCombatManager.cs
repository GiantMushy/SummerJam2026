using UnityEngine;

public class AiCombatManager : CombatManager
{
    private AiCharacterManager entity;

    [Header("Ai Combat Stats")]
    [SerializeField] private float attackCooldown = 2f;
    private bool isAttached = false;
    private bool isDead = false;
    private float attackTimer = 0f;
    private IDamageable target;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponent<AiCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
        entity.aiStatsManager.TriggerDeath         += OnDeath;
        entity.aiInteractionManager.TriggerAttached += OnAttached;
    }

    private void OnDestroy()
    {
        entity.aiStatsManager.TriggerDeath         -= OnDeath;
        entity.aiInteractionManager.TriggerAttached -= OnAttached;
    }

    private void OnDeath()    => isDead     = true;

    private void OnAttached()
    {
        isAttached = true;
        if (isDead) return;

        // First tick lands immediately on attach; interval ticks follow a cooldown later.
        PerformAttack();
        attackTimer = 0f;
    }

    public void TickCombat()
    {
        if (isAttached && !isDead)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                PerformAttack();
                attackTimer = 0f;
            }
        }
    }

    private void PerformAttack()
    {
        if (target == null)
            target = GameManager.instance?.player?.GetComponent<IDamageable>();

        target?.TakeDamage(entity.aiStatsManager.GetDamage());
    }
}