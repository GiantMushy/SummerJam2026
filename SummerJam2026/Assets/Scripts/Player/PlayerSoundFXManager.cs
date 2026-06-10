using UnityEngine;

public class PlayerSoundFXManager : CharacterSoundEffectManager
{
    [HideInInspector] protected CharacterManager character;

    [Header("Engine stuff")]
    [SerializeField] int currentRPM = 700;
    [SerializeField] AudioClip engineSoundFX;
    [SerializeField] float enginePitch;
    [SerializeField] bool throttleInput;
    [SerializeField] int rpmIncreaseRate = 2500;
    [SerializeField] int rpmDecreaseRate = 3000;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource engineAudioSource;

    protected override void Awake()
    {
        base.Awake();
        character = GetComponent<CharacterManager>();

        engineAudioSource.clip = engineSoundFX;
        engineAudioSource.loop = true;
        engineAudioSource.volume = 0.28f;
        engineAudioSource.Play();

        if (audioSource == null)
            Debug.LogWarning("AudioSource component is missing on " + gameObject.name);
        if (engineAudioSource == null)
            Debug.LogWarning("Engine AudioSource component is missing on " + gameObject.name);
        if (engineSoundFX == null)
            Debug.LogWarning("Engine sound FX is not assigned on " + gameObject.name);
    }


    public virtual void HandleEngineSoundFX()
    {
        if (throttleInput)
        {
            currentRPM = Mathf.RoundToInt(Mathf.Clamp(currentRPM + Time.deltaTime * rpmIncreaseRate, 700, 3000));
        }
        else
        {
            currentRPM = Mathf.RoundToInt(Mathf.Clamp(currentRPM - Time.deltaTime * rpmDecreaseRate, 700, 3000));
        }

        currentRPM = Random.Range(currentRPM - 50, currentRPM + 50); // add some random variation to RPM for a more natural sound

        // normalize RPM to a 0-1 range for pitch calculation
        enginePitch = Mathf.Lerp(0.5f, 1.2f, (currentRPM - 700) / 2300f); // 700 is idle RPM, 3000 is max RPM
        engineAudioSource.pitch = enginePitch;
    }


    public void SetThrottleInput(bool throttle)
    {
        throttleInput = throttle;
    }

}
