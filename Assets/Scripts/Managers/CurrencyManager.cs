using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks the player's Dream Coin balance and broadcasts changes so that
/// UI components can react without tight coupling.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Singleton
    // ──────────────────────────────────────────────────────────────

    public static CurrencyManager Instance { get; private set; }

    // ──────────────────────────────────────────────────────────────
    //  Inspector
    // ──────────────────────────────────────────────────────────────

    [SerializeField] private int startingCoins = 150;

    // ──────────────────────────────────────────────────────────────
    //  Events
    // ──────────────────────────────────────────────────────────────

    /// <summary>Fired whenever the coin balance changes. Passes the new balance.</summary>
    public UnityEvent<int> OnCoinsChanged = new UnityEvent<int>();

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    public int Coins { get; private set; }

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
    }

    private void Start()
    {
        Coins = startingCoins;
        OnCoinsChanged.Invoke(Coins);
    }

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Returns true and deducts <paramref name="amount"/> if the player can afford it.</summary>
    public bool TrySpend(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("[CurrencyManager] Spend amount must be positive.");
            return false;
        }

        if (Coins < amount)
            return false;

        Coins -= amount;
        OnCoinsChanged.Invoke(Coins);
        return true;
    }

    /// <summary>Add <paramref name="amount"/> Dream Coins to the balance.</summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[CurrencyManager] Reward amount must be greater than zero.");
            return;
        }

        Coins += amount;
        OnCoinsChanged.Invoke(Coins);
    }
}
