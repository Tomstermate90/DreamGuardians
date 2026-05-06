using UnityEngine;

/// <summary>
/// Fairy Godmother Guardian – a long-range support tower that fires magic
/// sparkles applying a slow debuff to enemies, reducing their movement speed.
/// </summary>
public class FairyGodmotherTower : Tower
{
    [Header("Fairy Godmother – Specific")]
    [SerializeField] private GameObject sparkleProjectilePrefab;
    [SerializeField] private Transform firePoint;

    [Tooltip("Multiplier applied to the enemy's speed (e.g. 0.5 = half speed).")]
    [SerializeField] [Range(0.1f, 0.9f)] private float slowMultiplier = 0.5f;

    [Tooltip("How long the slow effect lasts in seconds.")]
    [SerializeField] private float slowDuration = 2f;

    protected override void Start()
    {
        towerName   = "Fairy Godmother";
        cost        = 150;
        upgradeCost = 100;
        range       = 5f;
        fireRate    = 0.8f;
        damage      = 10;

        base.Start();
    }

    protected override void Shoot()
    {
        if (currentTarget == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject projGO = Instantiate(sparkleProjectilePrefab, spawnPos, Quaternion.identity);

        if (projGO.TryGetComponent(out Projectile proj))
        {
            proj.Initialise(currentTarget, damage);
            proj.ApplySlow(slowMultiplier, slowDuration);
        }
    }
}
