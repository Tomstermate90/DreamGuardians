using UnityEngine;

/// <summary>
/// Toy Soldier Guardian – a balanced mid-range tower with moderate damage and
/// fire rate, representing the bread-and-butter defensive unit.
/// </summary>
public class ToySoldierTower : Tower
{
    [Header("Toy Soldier – Specific")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override void Start()
    {
        towerName   = "Toy Soldier";
        cost        = 100;
        upgradeCost = 75;
        range       = 4f;
        fireRate    = 1f;
        damage      = 15;

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
