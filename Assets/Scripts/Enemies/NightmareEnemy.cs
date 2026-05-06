using UnityEngine;

/// <summary>
/// Concrete nightmare enemy – a standard shadowy creature that walks the path
/// toward the sleeping child. Serves as a template for more specialised enemies.
/// </summary>
public class NightmareEnemy : Enemy
{
    [Header("Nightmare – Specific")]
    [Tooltip("Optional particle effect played on death.")]
    [SerializeField] private GameObject deathVFXPrefab;

    protected override void Start()
    {
        enemyName   = "Shadow Nightmare";
        maxHealth   = 50;
        moveSpeed   = 2f;
        dreamDamage = 10;
        coinReward  = 20;

        base.Start();
    }

    protected override void Die()
    {
        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        base.Die();
    }
}
