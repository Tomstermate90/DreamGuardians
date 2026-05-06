using UnityEngine;

/// <summary>
/// Teddy Bear Guardian – a short-range, high-attack-speed melee-style tower that
/// fires plush projectiles at the nearest nightmare enemy.
/// </summary>
public class TeddyBearTower : Tower
{
    [Header("Teddy Bear – Specific")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override void Start()
    {
        // Set default stats if not overridden in Inspector.
        towerName   = "Teddy Bear";
        cost        = 75;
        upgradeCost = 50;
        range       = 2.5f;
        fireRate    = 1.5f;
        damage      = 8;

        base.Start();
    }

    protected override void Shoot()
    {
        if (currentTarget == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject projGO = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        if (projGO.TryGetComponent(out Projectile proj))
        {
            proj.Initialise(currentTarget, damage);
        }
    }
}
