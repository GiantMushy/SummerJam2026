using UnityEngine;

public class PlayerStatsManager : StatsManager
{
    private PlayerCharacterManager player;

    [Header("Player Specific Stats")]
    [SerializeField] float defaultTurnSpeed = 150f;
    private float turnSpeed;
    [SerializeField] float defaultFuelConsumptionRate = 1f;
    private float fuelConsumptionRate;
    [SerializeField] float defaultBoostingFuelConsumptionMultiplier = 2f;
    private float boostingFuelConsumptionMultiplier;
    [SerializeField] float defaultMaxFuel = 100f;
    private float maxFuel;
    [SerializeField] int defaultMaxPassengers = 4;
    private int maxPassengers;

    [Header("Gear Stats")]
    [SerializeField] float defaultGear1SpeedThreshold = 3f;
    private float gear1SpeedThreshold;
    [SerializeField] float defaultGear1Acceleration = 15f;
    private float gear1Acceleration;
    [SerializeField] float defaultGear2SpeedThreshold = 20f;
    private float gear2SpeedThreshold;
    [SerializeField] float defaultGear2Acceleration = 8f;
    private float gear2Acceleration;
    [SerializeField] float defaultGear3Acceleration = 4f;
    private float gear3Acceleration;

    [Header("Steering Stats")]
    [SerializeField] float defaultRearAxleOffset = 0.5f;
    private float rearAxleOffset;
    [SerializeField] float defaultOptimalTurnSpeed = 4f;
    private float optimalTurnSpeed;

    [Header("Drift Stats")]
    [SerializeField] float defaultMinFriction = 0.2f;
    private float minFriction;
    [SerializeField] float defaultDriftSpeedThreshold = 20f;
    private float driftSpeedThreshold;
    [SerializeField] float defaultDriftEntryTime = 0.2f;
    private float driftEntryTime;
    [SerializeField] float defaultDriftingMaxSpeed = 30f;
    private float driftingMaxSpeed;
    [SerializeField] float defaultFrictionDecayRate = 10f;
    private float frictionDecayRate;
    [SerializeField] float defaultFrictionRecoveryRate = 4f;
    private float frictionRecoveryRate;
    [SerializeField] float defaultReverseNeutralDelay = 0.4f;
    private float reverseNeutralDelay;
    [SerializeField] float defaultDriftAngularImpulse = 150f;
    private float driftAngularImpulse;
    [SerializeField] float defaultDriftTurnMultiplier = 1.5f;
    private float driftTurnMultiplier;


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
        turnSpeed                        = defaultTurnSpeed;
        fuelConsumptionRate              = defaultFuelConsumptionRate;
        boostingFuelConsumptionMultiplier = defaultBoostingFuelConsumptionMultiplier;
        maxFuel                          = defaultMaxFuel;
        maxPassengers                    = defaultMaxPassengers;
        gear1SpeedThreshold              = defaultGear1SpeedThreshold;
        gear1Acceleration                = defaultGear1Acceleration;
        gear2SpeedThreshold              = defaultGear2SpeedThreshold;
        gear2Acceleration                = defaultGear2Acceleration;
        gear3Acceleration                = defaultGear3Acceleration;
        rearAxleOffset                   = defaultRearAxleOffset;
        optimalTurnSpeed                 = defaultOptimalTurnSpeed;
        minFriction                      = defaultMinFriction;
        driftSpeedThreshold              = defaultDriftSpeedThreshold;
        driftEntryTime                   = defaultDriftEntryTime;
        driftingMaxSpeed                 = defaultDriftingMaxSpeed;
        frictionDecayRate                = defaultFrictionDecayRate;
        frictionRecoveryRate             = defaultFrictionRecoveryRate;
        reverseNeutralDelay              = defaultReverseNeutralDelay;
        driftAngularImpulse              = defaultDriftAngularImpulse;
        driftTurnMultiplier              = defaultDriftTurnMultiplier;
    }

    public float GetTurnSpeed()                                         => turnSpeed;
    public float GetFuelConsumptionRate()                               => fuelConsumptionRate;
    public float GetMaxFuel()                                           => maxFuel;
    public int   GetMaxPassengers()                                     => maxPassengers;
    public float GetGear1SpeedThreshold()                               => gear1SpeedThreshold;
    public float GetGear1Acceleration()                                 => gear1Acceleration;
    public float GetGear2SpeedThreshold()                               => gear2SpeedThreshold;
    public float GetGear2Acceleration()                                 => gear2Acceleration;
    public float GetGear3Acceleration()                                 => gear3Acceleration;
    public float GetRearAxleOffset()                                    => rearAxleOffset;
    public float GetOptimalTurnSpeed()                                  => optimalTurnSpeed;
    public float GetMinFriction()                                       => minFriction;
    public float GetDriftSpeedThreshold()                               => driftSpeedThreshold;
    public float GetDriftEntryTime()                                    => driftEntryTime;
    public float GetDriftingMaxSpeed()                                  => driftingMaxSpeed;
    public float GetFrictionDecayRate()                                 => frictionDecayRate;
    public float GetFrictionRecoveryRate()                              => frictionRecoveryRate;
    public float GetReverseNeutralDelay()                               => reverseNeutralDelay;
    public float GetDriftAngularImpulse()                               => driftAngularImpulse;
    public float GetDriftTurnMultiplier()                               => driftTurnMultiplier;
    public float GetBoostingFuelConsumptionMultiplier()                 => boostingFuelConsumptionMultiplier;

    public void IncreaseTurnSpeed(float amount)                         => turnSpeed += amount;
    public void IncreaseFuelConsumptionRate(float amount)               => fuelConsumptionRate += amount;
    public void IncreaseBoostingFuelConsumptionMultiplier(float amount) => boostingFuelConsumptionMultiplier += amount;
    public void IncreaseMaxFuel(float amount)                           => maxFuel += amount;
    public void IncreaseGear1SpeedThreshold(float amount)               => gear1SpeedThreshold += amount;
    public void IncreaseGear1Acceleration(float amount)                 => gear1Acceleration += amount;
    public void IncreaseGear2SpeedThreshold(float amount)               => gear2SpeedThreshold += amount;
    public void IncreaseGear2Acceleration(float amount)                 => gear2Acceleration += amount;
    public void IncreaseGear3Acceleration(float amount)                 => gear3Acceleration += amount;
    public void IncreaseRearAxleOffset(float amount)                    => rearAxleOffset += amount;
    public void IncreaseOptimalTurnSpeed(float amount)                  => optimalTurnSpeed += amount;
    public void IncreaseMinFriction(float amount)                       => minFriction += amount;
    public void IncreaseDriftSpeedThreshold(float amount)               => driftSpeedThreshold += amount;
    public void IncreaseDriftEntryTime(float amount)                    => driftEntryTime += amount;
    public void IncreaseDriftingMaxSpeed(float amount)                  => driftingMaxSpeed += amount;
    public void IncreaseFrictionDecayRate(float amount)                 => frictionDecayRate += amount;
    public void IncreaseFrictionRecoveryRate(float amount)              => frictionRecoveryRate += amount;
    public void IncreaseReverseNeutralDelay(float amount)               => reverseNeutralDelay += amount;
    public void IncreaseDriftAngularImpulse(float amount)               => driftAngularImpulse += amount;
    public void IncreaseDriftTurnMultiplier(float amount)               => driftTurnMultiplier += amount;

    public void DecreaseTurnSpeed(float amount)                         => turnSpeed -= amount;
    public void DecreaseFuelConsumptionRate(float amount)               => fuelConsumptionRate -= amount;
    public void DecreaseBoostingFuelConsumptionMultiplier(float amount) => boostingFuelConsumptionMultiplier -= amount;
    public void DecreaseMaxFuel(float amount)                           => maxFuel -= amount;
    public void DecreaseGear1SpeedThreshold(float amount)               => gear1SpeedThreshold -= amount;
    public void DecreaseGear1Acceleration(float amount)                 => gear1Acceleration -= amount;
    public void DecreaseGear2SpeedThreshold(float amount)               => gear2SpeedThreshold -= amount;
    public void DecreaseGear2Acceleration(float amount)                 => gear2Acceleration -= amount;
    public void DecreaseGear3Acceleration(float amount)                 => gear3Acceleration -= amount;
    public void DecreaseRearAxleOffset(float amount)                    => rearAxleOffset -= amount;
    public void DecreaseOptimalTurnSpeed(float amount)                  => optimalTurnSpeed -= amount;
    public void DecreaseMinFriction(float amount)                       => minFriction -= amount;
    public void DecreaseDriftSpeedThreshold(float amount)               => driftSpeedThreshold -= amount;
    public void DecreaseDriftEntryTime(float amount)                    => driftEntryTime -= amount;
    public void DecreaseDriftingMaxSpeed(float amount)                  => driftingMaxSpeed -= amount;
    public void DecreaseFrictionDecayRate(float amount)                 => frictionDecayRate -= amount;
    public void DecreaseFrictionRecoveryRate(float amount)              => frictionRecoveryRate -= amount;
    public void DecreaseReverseNeutralDelay(float amount)               => reverseNeutralDelay -= amount;
    public void DecreaseDriftAngularImpulse(float amount)               => driftAngularImpulse -= amount;
    public void DecreaseDriftTurnMultiplier(float amount)               => driftTurnMultiplier -= amount;
}
