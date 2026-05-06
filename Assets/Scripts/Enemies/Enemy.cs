using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all nightmare enemies.
/// Moves along a series of waypoints and damages the Dream Meter on arrival.
/// </summary>
public class Enemy : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Inspector
    // ──────────────────────────────────────────────────────────────

    [Header("Enemy Stats")]
    [SerializeField] protected string enemyName = "Nightmare";
    [SerializeField] protected int maxHealth = 50;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected int dreamDamage = 10;  // damage dealt to Dream Meter on breakthrough
    [SerializeField] protected int coinReward = 20;

    [Header("Health Bar (optional)")]
    [SerializeField] private UnityEngine.UI.Slider healthBar;

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    protected int currentHealth;

    private int waypointIndex = 0;
    private bool slowed = false;
    private float originalSpeed;

    /// <summary>0 … 1 progress along the entire path (used for tower targeting priority).</summary>
    public float PathProgress => waypoints == null || waypoints.Length == 0
        ? 0f
        : (float)waypointIndex / waypoints.Length;

    private Transform[] waypoints;

    // Cached scene references (set in Start to avoid repeated FindObjectOfType calls).
    private DreamMeter cachedDreamMeter;
    private WaveManager cachedWaveManager;

    // ──────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────────────────────────

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        originalSpeed = moveSpeed;

        // Cache scene references once to avoid repeated FindObjectOfType calls.
        var path = FindObjectOfType<Waypoints>();
        if (path != null)
            waypoints = path.GetWaypoints();

        cachedDreamMeter  = FindObjectOfType<DreamMeter>();
        cachedWaveManager = FindObjectOfType<WaveManager>();

        EnemyTracker.Register(this);

        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    protected virtual void Update()
    {
        MoveAlongPath();
    }

    private void OnDestroy()
    {
        EnemyTracker.Unregister(this);
    }

    // ──────────────────────────────────────────────────────────────
    //  Movement
    // ──────────────────────────────────────────────────────────────

    private void MoveAlongPath()
    {
        if (waypoints == null || waypointIndex >= waypoints.Length)
        {
            ReachEnd();
            return;
        }

        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            waypointIndex++;
    }

    private void ReachEnd()
    {
        if (cachedDreamMeter != null)
            cachedDreamMeter.TakeDamage(dreamDamage);

        if (cachedWaveManager != null)
            cachedWaveManager.OnEnemyRemoved();

        Destroy(gameObject);
    }

    // ──────────────────────────────────────────────────────────────
    //  Damage
    // ──────────────────────────────────────────────────────────────

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        CurrencyManager.Instance.AddCoins(coinReward);

        if (cachedWaveManager != null)
            cachedWaveManager.OnEnemyRemoved();

        Destroy(gameObject);
    }

    // ──────────────────────────────────────────────────────────────
    //  Status Effects
    // ──────────────────────────────────────────────────────────────

    /// <summary>Apply a movement slow for a limited duration.</summary>
    public void ApplySlow(float multiplier, float duration)
    {
        if (!slowed)
            StartCoroutine(SlowRoutine(multiplier, duration));
    }

    private IEnumerator SlowRoutine(float multiplier, float duration)
    {
        slowed = true;
        moveSpeed = originalSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
        slowed = false;
    }
}
