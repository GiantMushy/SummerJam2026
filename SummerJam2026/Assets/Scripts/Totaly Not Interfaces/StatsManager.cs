using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public CharacterManager character;

    [Header("Movement Stats")]
    public float maxSpeed;
    public float acceleration;
    public float autoDeceleration;
    public float manualDeceleration;
    public float sprintMultiplier;

    [Header("Combat Stats")]
    public float maxHealth = 100f;
    public float armour;
    public float damage;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public float GetMaxSpeed()                              => maxSpeed;
    public float GetAcceleration()                          => acceleration;
    public float GetAutoDeceleration()                      => autoDeceleration;
    public float GetManualDeceleration()                    => manualDeceleration;
    public float GetSprintMultiplier()                      => sprintMultiplier;
    public float GetMaxHealth()                             => maxHealth;
    public float GetArmour()                                => armour;
    public float GetDamage()                                => damage;

    public void IncreaseMaxSpeed(float amount)              => maxSpeed += amount;
    public void IncreaseAcceleration(float amount)          => acceleration += amount;
    public void IncreaseAutoDeceleration(float amount)      => autoDeceleration += amount;
    public void IncreaseManualDeceleration(float amount)    => manualDeceleration += amount;
    public void IncreaseSprintMultiplier(float amount)      => sprintMultiplier += amount;
    public void IncreaseMaxHealth(float amount)             => maxHealth += amount;
    public void IncreaseArmour(float amount)                => armour += amount;
    public void IncreaseDamage(float amount)                => damage += amount;

    public void DecreaseMaxSpeed(float amount)              => maxSpeed -= amount;
    public void DecreaseAcceleration(float amount)          => acceleration -= amount;
    public void DecreaseAutoDeceleration(float amount)      => autoDeceleration -= amount;
    public void DecreaseManualDeceleration(float amount)    => manualDeceleration -= amount;
    public void DecreaseSprintMultiplier(float amount)      => sprintMultiplier -= amount;
    public void DecreaseMaxHealth(float amount)             => maxHealth -= amount;
    public void DecreaseArmour(float amount)                => armour -= amount;
    public void DecreaseDamage(float amount)                => damage -= amount;
}