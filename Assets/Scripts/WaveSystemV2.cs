using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveAction
{
    public string name;
    public float delay;
    public Transform prefab;
    public int spawnCount;
}

[System.Serializable]
public class Wave
{
    public string name;
    public List<WaveAction> actions;
}

public class WaveSystemV2 : MonoBehaviour
{
    public List<Wave> waves;
    private Wave m_CurrentWave;
    public Wave CurrentWave { get { return m_CurrentWave; } }

    private List<GameObject> activeEnemies = new List<GameObject>(); // Track spawned enemies
    private int currentWaveIndex = 0; // Track current wave index
    private int waveRepeatCount = 0; // Track how many times the last wave has been repeated
    private int enemyCount = 0; // Track enemy count 
    private int maxEnemyCount = 0; // Get the max count of enemies
    private int globalWaveIndex = 0;

    public Transform towerPos; //Save tower position 

    public delegate void WaveStartedHandler();
    public static event WaveStartedHandler OnWaveStarted;

    public delegate void WaveCompletedHandler();
    public static event WaveCompletedHandler OnWaveCompleted;

    public void spawnWaves()
    {
        StartCoroutine(spawnEnemies());
    }
    public IEnumerator spawnEnemies()
    {
        OnWaveStarted?.Invoke();
        globalWaveIndex++;
   
        // Get current wave
        if (currentWaveIndex >= waves.Count)
        {
            // After predefined waves, reuse the last wave and increase difficulty
            m_CurrentWave = GetScaledWave(waves[waves.Count - 1], waveRepeatCount + 1);
            waveRepeatCount++;
        }
        else
        {
            m_CurrentWave = waves[currentWaveIndex];
        }

        // Instantiate every action in the current wave
        foreach (WaveAction A in m_CurrentWave.actions)
        {
            if (A.delay > 0)
                yield return new WaitForSeconds(A.delay);

            if (A.prefab != null && A.spawnCount > 0)
            {
                for (int i = 0; i < A.spawnCount; i++)
                {
                    Vector3 spawnPosition = GetSpawnPositionEnemies();
                    GameObject enemy = Instantiate(A.prefab, spawnPosition, Quaternion.identity).gameObject;
                    if (currentWaveIndex >= waves.Count)
                    {
                        ApplyDifficulty(enemy, waveRepeatCount);
                    }
                    activeEnemies.Add(enemy);
                    enemyCount++;
                }
            }
        }

        maxEnemyCount = enemyCount;

        // Wait until all enemies are defeated
        yield return new WaitUntil(() => activeEnemies.Count == 0);

        

        // Move to the next wave
        if (currentWaveIndex < waves.Count)
        {
            currentWaveIndex++;
        }

        transform.position = GetNewSpawnPointPosition();
        OnWaveCompleted?.Invoke(); // Notify the GameManager

    }

    void Update()
    {
        // Remove destroyed enemies from the activeEnemies list
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                activeEnemies.RemoveAt(i);
                enemyCount--;
            }
        }
    }

    // Get a random spawn position around the spawn point
    Vector3 GetSpawnPositionEnemies()
    {
        float angle = Random.Range(0, 360); // Random angle around the tower
        float radius = 10f; // Distance from the spawn point
        Vector3 spawnPosition = transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
        return spawnPosition;
    }

    // Get a random spawn position around the tower
    Vector3 GetNewSpawnPointPosition()
    {
        float angle = Random.Range(0, 360); // Random angle around the tower
        float radius = 50f; // Distance from the tower
        Vector3 spawnPosition = towerPos.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
        return spawnPosition;
    }

    // Scale the last wave to increase difficulty
    Wave GetScaledWave(Wave originalWave, int repeatCount)
    {
        Wave scaledWave = new Wave
        {
            name = originalWave.name + " (x" + repeatCount + ")",
            actions = new List<WaveAction>()
        };

        foreach (WaveAction action in originalWave.actions)
        {
            WaveAction scaledAction = new WaveAction
            {
                name = action.name,
                delay = action.delay,
                prefab = action.prefab,
                spawnCount = action.spawnCount + repeatCount * 2, // Increase spawn count
            };
            scaledWave.actions.Add(scaledAction);
        }

        return scaledWave;
    }

    // Apply difficulty scaling to enemies
    void ApplyDifficulty(GameObject enemy, int repeatCount)
    {
        EnemyBehaviour stats = enemy.GetComponent<EnemyBehaviour>();
        if (stats != null)
        {
            stats.SetMaxHealth(stats.GetMaxHealth() + repeatCount * 10); // Increase health
        }
    }
    public int getGlobalWaveIndex()
    {
        return globalWaveIndex;
    }
   public int getEnemyCount() {
        return enemyCount;
    }
    public int getMaxEnemyCount()
    {
        return maxEnemyCount;
    }
}
