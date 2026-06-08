using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    public float gameTime = 0f;
    public float score = 0f;

    // Best survival time across restarts. Static so a scene reload (restart) keeps it;
    // -1 means no run has finished yet ("N/A"). Resets when the application closes.
    private static float bestTime = -1f;

    public bool HasBestTime => bestTime >= 0f;
    public float GetBestTime() => bestTime;

    /// <summary>Records a finished run, keeping it if it beats the current best. Returns true on a new best.</summary>
    public bool RecordRunTime(float runTime)
    {
        if (runTime <= bestTime) return false;
        bestTime = runTime;
        return true;
    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }
}
