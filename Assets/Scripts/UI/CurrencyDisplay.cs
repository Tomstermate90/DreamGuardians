using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listens to <see cref="CurrencyManager.OnCoinsChanged"/> and updates the
/// on-screen Dream Coin counter accordingly.
/// </summary>
public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private Text coinText;
    [SerializeField] private string prefix = "Dream Coins: ";

    private void Start()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged.AddListener(UpdateDisplay);
            UpdateDisplay(CurrencyManager.Instance.Coins);
        }
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged.RemoveListener(UpdateDisplay);
    }

    private void UpdateDisplay(int coins)
    {
        if (coinText != null)
            coinText.text = $"{prefix}{coins}";
    }
}
