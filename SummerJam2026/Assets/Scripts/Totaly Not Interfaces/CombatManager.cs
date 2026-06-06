using UnityEngine;

public class CombatManager : MonoBehaviour, IDamageable
{
    [HideInInspector] public CharacterManager character;

    private float health;
    private bool hasDied;
    private bool healthInitialized;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {

    }

    // Lazily pull from stats so we don't depend on StatsManager.Start() running first.
    private void EnsureHealthInitialized()
    {
        if (healthInitialized) return;
        health = character.stats.GetMaxHealth();
        healthInitialized = true;
    }

    public void TakeDamage(float damage)
    {
        if (hasDied) return;
        EnsureHealthInitialized();

        health = Mathf.Max(0f, health - damage);

        if (health <= 0f)
        {
            hasDied = true;
            character.Die();
        }
    }

    public float GetHealth()
    {
        EnsureHealthInitialized();
        return health;
    }

    public void Heal(float amount)
    {
        if (hasDied) return;
        EnsureHealthInitialized();

        health = Mathf.Min(GetMaxHealth(), health + amount);
    }

    public float GetMaxHealth() => character.stats.GetMaxHealth();
}