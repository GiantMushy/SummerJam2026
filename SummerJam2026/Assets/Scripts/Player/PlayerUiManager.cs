using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUiManager : UiManager
{
    private enum GameState { WaitingToStart, Running, Dead }

    private PlayerCharacterManager player;
    [SerializeField] private CanvasGroup movementInstructionsPrefab;
    [SerializeField] private CanvasGroup deathDisplayPrefab;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimerText;
    [SerializeField] private TextMeshProUGUI deathTimerText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image fuelBar;

    [SerializeField] private float instructionsFadeDuration = 0.3f;

    private GameState state;
    private float elapsedTime;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerCharacterManager>();
    }

    protected override void Start()
    {
        base.Start();

        state = GameState.WaitingToStart;
        elapsedTime = 0f;

        movementInstructionsPrefab.alpha = 1f;

        deathDisplayPrefab.alpha = 0f;
        deathDisplayPrefab.interactable = false;
        deathDisplayPrefab.blocksRaycasts = false;

        timerText.text = FormatTime(0f);
        bestTimerText.text = FormatBestTime();
    }

    private void Update()
    {
        UpdateStatusBars();

        switch (state)
        {
            case GameState.WaitingToStart:
                if (InputManager.instance.verticalInput > 0f)
                    BeginRun();
                break;

            case GameState.Running:
                elapsedTime += Time.deltaTime;
                timerText.text = FormatTime(elapsedTime);
                break;

            case GameState.Dead:
                if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
                    RestartGame();
                break;
        }
    }

    private void UpdateStatusBars()
    {
        float health = player.playerCombatManager.GetHealth();
        float maxHealth = player.playerCombatManager.GetMaxHealth();
        healthBar.fillAmount = health / maxHealth;

        float fuel = player.playerInventoryManager.GetFuel();
        float maxFuel = player.playerStatsManager.GetMaxFuel();
        fuelBar.fillAmount = fuel / maxFuel;
    }

    private void BeginRun()
    {
        state = GameState.Running;
        elapsedTime = 0f;
        StartCoroutine(FadeOut(movementInstructionsPrefab, instructionsFadeDuration));
    }

    public void OnPlayerDeath()
    {
        state = GameState.Dead;

        GameManager.instance.RecordRunTime(elapsedTime);

        deathTimerText.text = FormatTime(elapsedTime);
        bestTimerText.text = FormatBestTime();

        deathDisplayPrefab.alpha = 1f;
        deathDisplayPrefab.blocksRaycasts = true;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string FormatBestTime()
    {
        return GameManager.instance.HasBestTime ? FormatTime(GameManager.instance.GetBestTime()) : "N/A";
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        float remainder = seconds - minutes * 60f;
        return $"{minutes:00}:{remainder:00.00}";
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
