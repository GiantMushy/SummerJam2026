using UnityEngine;

public class PlayerInventoryManager : InventoryManager
{
    private PlayerCharacterManager player;
    private PlayerStatsManager statsManager;
    private float fuel;
    private float armour;
    private int bulletAmmo;
    private int grenadeAmmo;
    private int landmineAmmo;
    private int numOfPassengers;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
        statsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Start()
    {
        base.Start();
        fuel = 100f;
        armour = 0f;
        bulletAmmo = 0;
        grenadeAmmo = 0;
        landmineAmmo = 0;
        numOfPassengers = 0;
    }

    public float GetFuel() => fuel;
    public float GetArmour() => armour;
    public int GetBulletAmmo() => bulletAmmo;
    public int GetGrenadeAmmo() => grenadeAmmo;
    public int GetLandmineAmmo() => landmineAmmo;

    public bool ConsumeFuel(float amount) {
        if (fuel >= amount) {
            fuel -= amount;
            return true;
        } else {
            return false;
        }
    }
    public bool RemoveArmour(float amount) {
        if (armour >= amount) {
            armour -= amount;
            return true;
        } else {
            return false;
        }
    }
    public bool ConsumeBulletAmmo() {
        if (bulletAmmo > 0) {
            bulletAmmo -= 1;
            return true;
        } else {
            return false;
        }
    }
    public bool ConsumeGrenadeAmmo() {
        if (grenadeAmmo > 0) {
            grenadeAmmo -= 1;
            return true;
        } else {
            return false;
        }
    }
    public bool ConsumeLandmineAmmo() {
        if (landmineAmmo > 0) {
            landmineAmmo -= 1;
            return true;
        } else {
            return false;
        }
    }

    public void AddFuel(float amount) {
        if (fuel + amount > statsManager.GetMaxFuel()) {
            fuel = statsManager.GetMaxFuel();
        } else {
            fuel += amount;
        }
    }
    public void AddArmour(float amount) => armour += amount;
    public void AddBulletAmmo(int amount) => bulletAmmo += amount;
    public void AddGrenadeAmmo(int amount) => grenadeAmmo += amount;
    public void AddLandmineAmmo(int amount) => landmineAmmo += amount;
    public void AddPassenger() {
        if (numOfPassengers < statsManager.GetMaxPassengers()) {
            numOfPassengers++;
        }
    }

    public void ResetInventory() {
        fuel = 100f;
        armour = 0f;
        bulletAmmo = 0;
        grenadeAmmo = 0;
        landmineAmmo = 0;
        numOfPassengers = 0;
    }
}