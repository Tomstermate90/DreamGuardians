using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract base class for all Dream Guardian towers.
/// Handles target acquisition, rotation toward the target, and a generic
/// fire-rate loop. Concrete tower types override <see cref="Shoot"/> to
/// define their specific attack behaviour.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public abstract class Tower : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Inspector – shared stats
    // ──────────────────────────────────────────────────────────────

    [Header("Tower Stats")]
    [SerializeField] protected string towerName = "Guardian";
    [SerializeField] protected int cost = 100;
    [SerializeField] protected int upgradeCost = 75;
    [SerializeField] protected float range = 3f;
    [SerializeField] protected float fireRate = 1f;   // shots per second
    [SerializeField] protected int damage = 10;

    [Header("References")]
    [SerializeField] protected Transform rotatingHead;   // the sprite part that rotates

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    protected Enemy currentTarget;
    private float fireCooldown;
    private int upgradeLevel = 0;

    public int Cost => cost;
    public int UpgradeCost => upgradeCost;
    public int UpgradeLevel => upgradeLevel;

    // ──────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────────────────────────

    protected virtual void Start()
    {
        // Size the range collider to match the serialized range value.
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = range;
    }

    protected virtual void Update()
    {
        if (currentTarget == null)
        {
            FindNewTarget();
            return;
        }

        AimAtTarget();

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;
            Shoot();
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Abstract / Virtual
    // ──────────────────────────────────────────────────────────────

    /// <summary>Subclasses implement their specific attack (projectile, area, etc.).</summary>
    protected abstract void Shoot();

    /// <summary>Apply upgrade effects. Call base to increment the level counter.</summary>
    public virtual void Upgrade()
    {
        if (!CurrencyManager.Instance.TrySpend(upgradeCost))
        {
            Debug.Log($"[Tower] Not enough coins to upgrade {towerName}.");
            return;
        }

        upgradeLevel++;
        damage = Mathf.RoundToInt(damage * 1.25f);
        range  *= 1.1f;
        GetComponent<CircleCollider2D>().radius = range;
        Debug.Log($"[Tower] {towerName} upgraded to level {upgradeLevel}.");
    }

    // ──────────────────────────────────────────────────────────────
    //  Targeting
    // ──────────────────────────────────────────────────────────────

    private void FindNewTarget()
    {
        // Simple O(n) scan – sufficient for 2D tower defense at this scale.
        Enemy best = null;
        float bestProgress = float.MinValue;

        foreach (Enemy e in EnemyTracker.ActiveEnemies)
        {
            if (e == null) continue;
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist > range) continue;

            // Prefer the enemy that is furthest along the path.
            if (e.PathProgress > bestProgress)
            {
                bestProgress = e.PathProgress;
                best = e;
            }
        }

        currentTarget = best;
    }

    private void AimAtTarget()
    {
        if (rotatingHead == null) return;

        Vector2 dir = (currentTarget.transform.position - rotatingHead.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rotatingHead.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // ──────────────────────────────────────────────────────────────
    //  Trigger callbacks for range detection
    // ──────────────────────────────────────────────────────────────

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentTarget != null && other.gameObject == currentTarget.gameObject)
            currentTarget = null;
    }

    // ──────────────────────────────────────────────────────────────
    //  Gizmos
    // ──────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
