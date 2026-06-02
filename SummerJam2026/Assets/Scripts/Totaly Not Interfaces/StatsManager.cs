using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [HideInInspector] public CharacterManager character;

    [Header("Movement Stats")]
    [SerializeField] float defaultMaxSpeed = 10f;
    private float maxSpeed;
    [SerializeField] float defaultMinSpeed = -3f;
    private float minSpeed;
    [SerializeField] float defaultAcceleration = 5f;
    private float acceleration;
    [SerializeField] float defaultAutoDeceleration = 2f;
    private float autoDeceleration;
    [SerializeField] float defaultManualDeceleration = 3f;
    private float manualDeceleration;
    [SerializeField] float defaultSprintSpeedMultiplier = 1.5f;
    private float sprintSpeedMultiplier;

    [Header("Combat Stats")]
    [SerializeField] float defaultMaxHealth = 100f;
    private float maxHealth;
    [SerializeField] float defaultArmour = 0f;
    private float armour;
    [SerializeField] float defaultDamage = 10f;
    private float damage;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        SetInitialStats();   
    }

    protected virtual void SetInitialStats()
    {
        maxSpeed = defaultMaxSpeed;
        minSpeed = defaultMinSpeed;
        acceleration = defaultAcceleration;
        autoDeceleration = defaultAutoDeceleration;
        manualDeceleration = defaultManualDeceleration;
        sprintSpeedMultiplier = defaultSprintSpeedMultiplier;
        maxHealth = defaultMaxHealth;
        armour = defaultArmour;
        damage = defaultDamage;
    }

    protected virtual void ResetStats()
    {
        SetInitialStats();
    }

    public virtual float GetMaxSpeed()                              => maxSpeed;
    public virtual float GetMinSpeed()                              => minSpeed;
    public virtual float GetAcceleration()                          => acceleration;
    public virtual float GetAutoDeceleration()                      => autoDeceleration;
    public virtual float GetManualDeceleration()                    => manualDeceleration;
    public virtual float GetSprintSpeedMultiplier()                 => sprintSpeedMultiplier;
    public virtual float GetMaxHealth()                             => maxHealth;
    public virtual float GetArmour()                                => armour;
    public virtual float GetDamage()                                => damage;

    public virtual void IncreaseMaxSpeed(float amount)              => maxSpeed += amount;
    public virtual void IncreaseMinSpeed(float amount)              => minSpeed += amount;
    public virtual void IncreaseAcceleration(float amount)          => acceleration += amount;
    public virtual void IncreaseAutoDeceleration(float amount)      => autoDeceleration += amount;
    public virtual void IncreaseManualDeceleration(float amount)    => manualDeceleration += amount;
    public virtual void IncreaseSprintSpeedMultiplier(float amount) => sprintSpeedMultiplier += amount;
    public virtual void IncreaseMaxHealth(float amount)             => maxHealth += amount;
    public virtual void IncreaseArmour(float amount)                => armour += amount;
    public virtual void IncreaseDamage(float amount)                => damage += amount;

    public virtual void DecreaseMaxSpeed(float amount)              => maxSpeed -= amount;
    public virtual void DecreaseMinSpeed(float amount)              => minSpeed -= amount;
    public virtual void DecreaseAcceleration(float amount)          => acceleration -= amount;
    public virtual void DecreaseAutoDeceleration(float amount)      => autoDeceleration -= amount;
    public virtual void DecreaseManualDeceleration(float amount)    => manualDeceleration -= amount;
    public virtual void DecreaseSprintSpeedMultiplier(float amount) => sprintSpeedMultiplier -= amount;
    public virtual void DecreaseMaxHealth(float amount)             => maxHealth -= amount;
    public virtual void DecreaseArmour(float amount)                => armour -= amount;
    public virtual void DecreaseDamage(float amount)                => damage -= amount;
}