using UnityEngine;

public class PlayerStatsManager : StatsManager
{
    private PlayerCharacterManager player;

    [Header("Player Specific Stats")]
    private float turnSpeed;
    [SerializeField] float defaultTurnSpeed = 100f;
    private float fuelConsumptionRate;
    [SerializeField] float defaultFuelConsumptionRate = 1f;
    private float boostingFuelConsumptionMultiplier;
    [SerializeField] float defaultBoostingFuelConsumptionMultiplier = 2f;
    private float maxFuel;
    [SerializeField] float defaultMaxFuel = 100f;
    private int maxPassengers;
    [SerializeField] int defaultMaxPassengers = 4;
    private float tireFriction;
    [SerializeField] float defaultTireFriction = 1f;


    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void SetInitialStats()
    {
        base.SetInitialStats();
        turnSpeed = defaultTurnSpeed;
        fuelConsumptionRate = defaultFuelConsumptionRate;
        boostingFuelConsumptionMultiplier = defaultBoostingFuelConsumptionMultiplier;
        maxFuel = defaultMaxFuel;
        maxPassengers = defaultMaxPassengers;
        tireFriction = defaultTireFriction;
    }
    
    public float GetTurnSpeed()                             => turnSpeed;
    public float GetFuelConsumptionRate()                   => fuelConsumptionRate;
    public float GetMaxFuel()                               => maxFuel;
    public int GetMaxPassengers()                           => maxPassengers;
    public float GetTireFriction()                          => tireFriction;
    public float GetBoostingFuelConsumptionMultiplier()     => boostingFuelConsumptionMultiplier;

    public void IncreaseTurnSpeed(float amount)             => turnSpeed += amount;
    public void IncreaseFuelConsumptionRate(float amount)   => fuelConsumptionRate += amount;
    public void IncreaseBoostingFuelConsumptionMultiplier(float amount) => boostingFuelConsumptionMultiplier += amount;
    public void IncreaseMaxFuel(float amount)               => maxFuel += amount;
    public void IncreaseTireFriction(float amount)          => tireFriction += amount;

    public void DecreaseTurnSpeed(float amount)             => turnSpeed -= amount;
    public void DecreaseFuelConsumptionRate(float amount)   => fuelConsumptionRate -= amount;
    public void DecreaseBoostingFuelConsumptionMultiplier(float amount) => boostingFuelConsumptionMultiplier -= amount;
    public void DecreaseMaxFuel(float amount)               => maxFuel -= amount;
    public void DecreaseTireFriction(float amount)          => tireFriction -= amount;
}