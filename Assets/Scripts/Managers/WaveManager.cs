using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns waves of nightmare enemies along the path and notifies
/// <see cref="GameManager"/> when a wave has been fully defeated.
/// </summary>
public class WaveManager : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Data
    // ──────────────────────────────────────────────────────────────

    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave";
        public List<EnemySpawnEntry> spawnEntries = new List<EnemySpawnEntry>();
        [Tooltip("Seconds between consecutive spawns within this wave.")]
        public float spawnInterval = 1.5f;
    }

    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject enemyPrefab;
        [Tooltip("How many of this enemy type to spawn.")]
        public int count = 1;
    }

    // ──────────────────────────────────────────────────────────────
    //  Inspector
    // ──────────────────────────────────────────────────────────────

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [Tooltip("Delay in seconds before the very first wave begins.")]
    [SerializeField] private float firstWaveDelay = 3f;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    public int CurrentWaveIndex { get; private set; } = -1;
    public int TotalWaves => waves.Count;

    private int activeEnemies;
    private bool spawningInProgress;

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Advance to and start the next wave.</summary>
    public void StartNextWave()
    {
        CurrentWaveIndex++;

        if (CurrentWaveIndex >= waves.Count)
        {
            GameManager.Instance.TriggerVictory();
            return;
        }

        float delay = CurrentWaveIndex == 0 ? firstWaveDelay : 0f;
        StartCoroutine(SpawnWaveRoutine(waves[CurrentWaveIndex], delay));
    }

    /// <summary>Must be called by each enemy when it is destroyed or reaches the end.</summary>
    public void OnEnemyRemoved()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);

        if (!spawningInProgress && activeEnemies == 0)
        {
            GameManager.Instance.OnWaveCleared();
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Private
    // ──────────────────────────────────────────────────────────────

    private IEnumerator SpawnWaveRoutine(Wave wave, float initialDelay)
    {
        spawningInProgress = true;
        yield return new WaitForSeconds(initialDelay);

        foreach (EnemySpawnEntry entry in wave.spawnEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemyPrefab);
                activeEnemies++;
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        spawningInProgress = false;

        if (activeEnemies == 0)
        {
            GameManager.Instance.OnWaveCleared();
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[WaveManager] Enemy prefab is null – skipping spawn.");
            return;
        }

        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }
}
