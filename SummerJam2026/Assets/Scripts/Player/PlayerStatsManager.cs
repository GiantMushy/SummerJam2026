using UnityEngine;

public class PlayerStatsManager : StatsManager
{
    private PlayerCharacterManager player;

    [Header("Movement Stats")]
    private float maxSpeed;
    private float acceleration;
    private float autoDeceleration;
    private float manualDeceleration;
    private float sprintMultiplier;

    [Header("Combat Stats")]
    private float maxHealth = 100f;
    private float currentHealth = 100f;
    private float armour;
    private float damage;

    [Header("Player Specific Stats")]
    private float turnSpeed;
    private float fuelConsumptionRate;
    private float maxFuel;
    private int maxPassengers;


    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
    }
    

    public float GetMaxSpeed()                              => maxSpeed;
    public float GetAcceleration()                          => acceleration;
    public float GetAutoDeceleration()                      => autoDeceleration;
    public float GetManualDeceleration()                    => manualDeceleration;
    public float GetSprintMultiplier()                      => sprintMultiplier;
    public float GetMaxHealth()                             => maxHealth;
    public float GetArmour()                                => armour;
    public float GetDamage()                                => damage;
    public float GetTurnSpeed()                             => turnSpeed;
    public float GetFuelConsumptionRate()                   => fuelConsumptionRate;
    public float GetMaxFuel()                               => maxFuel;
    public int GetMaxPassengers()                            => maxPassengers;

    public void IncreaseMaxSpeed(float amount)              => maxSpeed += amount;
    public void IncreaseAcceleration(float amount)          => acceleration += amount;
    public void IncreaseAutoDeceleration(float amount)      => autoDeceleration += amount;
    public void IncreaseManualDeceleration(float amount)    => manualDeceleration += amount;
    public void IncreaseSprintMultiplier(float amount)      => sprintMultiplier += amount;
    public void IncreaseMaxHealth(float amount)             => maxHealth += amount;
    public void IncreaseArmour(float amount)                => armour += amount;
    public void IncreaseDamage(float amount)                => damage += amount;
    public void IncreaseTurnSpeed(float amount)            => turnSpeed += amount;
    public void IncreaseFuelConsumptionRate(float amount)  => fuelConsumptionRate += amount;
    public void IncreaseMaxFuel(float amount)              => maxFuel += amount;

    public void DecreaseMaxSpeed(float amount)              => maxSpeed -= amount;
    public void DecreaseAcceleration(float amount)          => acceleration -= amount;
    public void DecreaseAutoDeceleration(float amount)      => autoDeceleration -= amount;
    public void DecreaseManualDeceleration(float amount)    => manualDeceleration -= amount;
    public void DecreaseSprintMultiplier(float amount)      => sprintMultiplier -= amount;
    public void DecreaseMaxHealth(float amount)             => maxHealth -= amount;
    public void DecreaseArmour(float amount)                => armour -= amount;
    public void DecreaseDamage(float amount)                => damage -= amount;
    public void DecreaseTurnSpeed(float amount)            => turnSpeed -= amount;
    public void DecreaseFuelConsumptionRate(float amount)  => fuelConsumptionRate -= amount;
    public void DecreaseMaxFuel(float amount)              => maxFuel -= amount;
}