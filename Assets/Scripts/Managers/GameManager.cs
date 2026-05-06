using UnityEngine;

/// <summary>
/// Central game controller. Manages overall game state (playing, paused,
/// game-over) and acts as the single source of truth for references that
/// multiple systems need to reach.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Singleton
    // ──────────────────────────────────────────────────────────────

    public static GameManager Instance { get; private set; }

    // ──────────────────────────────────────────────────────────────
    //  Game State
    // ──────────────────────────────────────────────────────────────

    public enum GameState { MainMenu, Playing, Paused, WaveClear, GameOver, Victory }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    // ──────────────────────────────────────────────────────────────
    //  Inspector References
    // ──────────────────────────────────────────────────────────────

    [Header("Core Managers")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("UI")]
    [SerializeField] private DreamMeter dreamMeter;
    [SerializeField] private CurrencyDisplay currencyDisplay;

    // ──────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.Playing);
    }

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Transition to a new <see cref="GameState"/>.</summary>
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                waveManager.StartNextWave();
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.WaveClear:
                // Between-wave shop / upgrade phase handled by UI.
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                Debug.Log("[GameManager] Game Over – the nightmares broke through!");
                break;

            case GameState.Victory:
                Time.timeScale = 0f;
                Debug.Log("[GameManager] Victory – all waves defeated!");
                break;
        }
    }

    /// <summary>Called by <see cref="DreamMeter"/> when the meter hits zero.</summary>
    public void TriggerGameOver()
    {
        ChangeState(GameState.GameOver);
    }

    /// <summary>Called by <see cref="WaveManager"/> when all waves have been cleared.</summary>
    public void TriggerVictory()
    {
        ChangeState(GameState.Victory);
    }

    /// <summary>Called by <see cref="WaveManager"/> when a single wave ends.</summary>
    public void OnWaveCleared()
    {
        ChangeState(GameState.WaveClear);
    }
}
