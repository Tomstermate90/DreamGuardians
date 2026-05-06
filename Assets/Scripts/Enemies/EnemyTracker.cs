using System.Collections.Generic;

/// <summary>
/// Static registry of all currently active <see cref="Enemy"/> instances.
/// Towers poll this list to find targets without requiring expensive physics
/// overlap queries every frame.
/// </summary>
public static class EnemyTracker
{
    private static readonly HashSet<Enemy> _activeEnemies = new HashSet<Enemy>();

    /// <summary>Read-only view of all living enemies in the scene.</summary>
    public static IReadOnlyCollection<Enemy> ActiveEnemies => _activeEnemies;

    /// <summary>Called by <see cref="Enemy.Start"/> when an enemy enters the scene.</summary>
    public static void Register(Enemy enemy)
    {
        if (enemy != null)
            _activeEnemies.Add(enemy);
    }

    /// <summary>Called by <see cref="Enemy.OnDestroy"/> when an enemy is removed.</summary>
    public static void Unregister(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
    }

    /// <summary>Clear the registry (call between scenes to avoid stale references).</summary>
    public static void Clear()
    {
        _activeEnemies.Clear();
    }
}
