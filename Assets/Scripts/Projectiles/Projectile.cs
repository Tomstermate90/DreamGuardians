using UnityEngine;

/// <summary>
/// Generic homing projectile used by towers.
/// Moves toward its assigned target and deals damage on contact.
/// Supports an optional slow debuff (used by the Fairy Godmother tower).
/// </summary>
public class Projectile : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Inspector
    // ──────────────────────────────────────────────────────────────

    [SerializeField] private float speed = 8f;
    [SerializeField] private float maxLifetime = 5f;

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    private Enemy target;
    private int damage;

    private bool hasSlow;
    private float slowMultiplier;
    private float slowDuration;

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Set up the projectile after it is instantiated.</summary>
    public void Initialise(Enemy target, int damage)
    {
        this.target = target;
        this.damage = damage;
        Destroy(gameObject, maxLifetime);
    }

    /// <summary>Mark this projectile to apply a slow on hit.</summary>
    public void ApplySlow(float multiplier, float duration)
    {
        hasSlow       = true;
        slowMultiplier = multiplier;
        slowDuration   = duration;
    }

    // ──────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────────────────────────

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Face the direction of travel.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Close-enough collision check (avoids needing a collider on the projectile).
        if (Vector3.Distance(transform.position, target.transform.position) < 0.15f)
            HitTarget();
    }

    // ──────────────────────────────────────────────────────────────
    //  Private
    // ──────────────────────────────────────────────────────────────

    private void HitTarget()
    {
        if (target == null) return;

        target.TakeDamage(damage);

        if (hasSlow)
            target.ApplySlow(slowMultiplier, slowDuration);

        Destroy(gameObject);
    }
}
