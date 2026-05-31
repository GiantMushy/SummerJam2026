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

    [Header("Drift Stats")]
    private float minFriction;
    [SerializeField] float defaultMinFriction = 0.4f;
    private float driftSpeedThreshold;
    [SerializeField] float defaultDriftSpeedThreshold = 0.65f;
    private float driftEntryTime;
    [SerializeField] float defaultDriftEntryTime = 0.2f;
    private float driftingMaxSpeed;
    [SerializeField] float defaultDriftingMaxSpeed = 8f;
    private float frictionDecayRate;
    [SerializeField] float defaultFrictionDecayRate = 3f;
    private float frictionRecoveryRate;
    [SerializeField] float defaultFrictionRecoveryRate = 4f;
    private float reverseNeutralDelay;
    [SerializeField] float defaultReverseNeutralDelay = 0.4f;


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
        minFriction = defaultMinFriction;
        driftSpeedThreshold = defaultDriftSpeedThreshold;
        driftEntryTime = defaultDriftEntryTime;
        driftingMaxSpeed = defaultDriftingMaxSpeed;
        frictionDecayRate = defaultFrictionDecayRate;
        frictionRecoveryRate = defaultFrictionRecoveryRate;
        reverseNeutralDelay = defaultReverseNeutralDelay;
    }

    public float GetTurnSpeed()                                         => turnSpeed;
    public float GetFuelConsumptionRate()                               => fuelConsumptionRate;
    public float GetMaxFuel()                                           => maxFuel;
    public int   GetMaxPassengers()                                     => maxPassengers;
    public float GetMinFriction()                                       => minFriction;
    public float GetDriftSpeedThreshold()                               => driftSpeedThreshold;
    public float GetDriftEntryTime()                                    => driftEntryTime;
    public float GetDriftingMaxSpeed()                                  => driftingMaxSpeed;
    public float GetFrictionDecayRate()                                 => frictionDecayRate;
    public float GetFrictionRecoveryRate()                              => frictionRecoveryRate;
    public float GetReverseNeutralDelay()                               => reverseNeutralDelay;
    public float GetBoostingFuelConsumptionMultiplier()                 => boostingFuelConsumptionMultiplier;

    public void IncreaseTurnSpeed(float amount)                         => turnSpeed += amount;
    public void IncreaseFuelConsumptionRate(float amount)               => fuelConsumptionRate += amount;
    public void IncreaseBoostingFuelConsumptionMultiplier(float amount) => boostingFuelConsumptionMultiplier += amount;
    public void IncreaseMaxFuel(float amount)                           => maxFuel += amount;
    public void IncreaseMinFriction(float amount)                       => minFriction += amount;
    public void IncreaseDriftSpeedThreshold(float amount)               => driftSpeedThreshold += amount;
    public void IncreaseDriftEntryTime(float amount)                    => driftEntryTime += amount;
    public void IncreaseDriftingMaxSpeed(float amount)                  => driftingMaxSpeed += amount;
    public void IncreaseFrictionDecayRate(float amount)                 => frictionDecayRate += amount;
    public void IncreaseFrictionRecoveryRate(float amount)              => frictionRecoveryRate += amount;
    public void IncreaseReverseNeutralDelay(float amount)               => reverseNeutralDelay += amount;

    public void DecreaseTurnSpeed(float amount)                         => turnSpeed -= amount;
    public void DecreaseFuelConsumptionRate(float amount)               => fuelConsumptionRate -= amount;
    public void DecreaseBoostingFuelConsumptionMultiplier(float amount) => boostingFuelConsumptionMultiplier -= amount;
    public void DecreaseMaxFuel(float amount)                           => maxFuel -= amount;
    public void DecreaseMinFriction(float amount)                       => minFriction -= amount;
    public void DecreaseDriftSpeedThreshold(float amount)               => driftSpeedThreshold -= amount;
    public void DecreaseDriftEntryTime(float amount)                    => driftEntryTime -= amount;
    public void DecreaseDriftingMaxSpeed(float amount)                  => driftingMaxSpeed -= amount;
    public void DecreaseFrictionDecayRate(float amount)                 => frictionDecayRate -= amount;
    public void DecreaseFrictionRecoveryRate(float amount)              => frictionRecoveryRate -= amount;
    public void DecreaseReverseNeutralDelay(float amount)               => reverseNeutralDelay -= amount;
}
