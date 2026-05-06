using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the child's Dream Meter – the game's equivalent of a lives /
/// health bar.  When it reaches zero the nightmares have won and the game ends.
/// </summary>
public class DreamMeter : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Inspector
    // ──────────────────────────────────────────────────────────────

    [SerializeField] private int maxDreamPower = 100;

    [Header("UI References")]
    [SerializeField] private Slider dreamSlider;
    [SerializeField] private Text   dreamValueText;    // optional numeric label

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    public int CurrentDreamPower { get; private set; }

    // ──────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────────────────────────

    private void Start()
    {
        CurrentDreamPower = maxDreamPower;
        RefreshUI();
    }

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Reduce the Dream Meter by <paramref name="amount"/>.</summary>
    public void TakeDamage(int amount)
    {
        CurrentDreamPower = Mathf.Max(0, CurrentDreamPower - amount);
        RefreshUI();

        if (CurrentDreamPower <= 0)
            GameManager.Instance.TriggerGameOver();
    }

    /// <summary>Restore some dream power (e.g. end-of-wave bonus).</summary>
    public void Restore(int amount)
    {
        CurrentDreamPower = Mathf.Min(maxDreamPower, CurrentDreamPower + amount);
        RefreshUI();
    }

    // ──────────────────────────────────────────────────────────────
    //  Private
    // ──────────────────────────────────────────────────────────────

    private void RefreshUI()
    {
        if (dreamSlider != null)
        {
            dreamSlider.minValue = 0;
            dreamSlider.maxValue = maxDreamPower;
            dreamSlider.value    = CurrentDreamPower;
        }

        if (dreamValueText != null)
            dreamValueText.text = $"{CurrentDreamPower} / {maxDreamPower}";
    }
}
