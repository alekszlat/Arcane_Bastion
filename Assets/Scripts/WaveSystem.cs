using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
enum WaveSpawnerType
{
    GoblinSpawner,
    SkeletonSpawner,
    TrollSpawner,
    MixedSpawner
}

public class EnemySpawn
{
    GameObject enemyPrefab;
    int enemyCount;
    EnemySpawn(GameObject enemyPrefab,int enemyCount)
    {
        this.enemyPrefab = enemyPrefab;
        this.enemyCount = enemyCount;
    }

}
public class WaveSystem : MonoBehaviour
{
  
    [SerializeField] GameObject goblin;
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject troll;
    public static WaveSystem instance;
    private GameManager gameManager;
    private int sumOfEnemiesCount;
    WaveSpawnerType waveSpawnerType;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
  
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

   
    public void spawnWaves()
    {
        if (waveSpawnerType == WaveSpawnerType.GoblinSpawner)
        {

        }
        if(waveSpawnerType == WaveSpawnerType.SkeletonSpawner)
        {

        }
        if (waveSpawnerType == WaveSpawnerType.TrollSpawner)
        {

        }
        //   StartCoroutine(spawnEnemiesCoroutine());
    }
    public IEnumerator spawnEnemiesCoroutine(GameObject enemyPrefab,int enemyCount, float spawningCooldown)
    {
            for (int i = 0; i < enemyCount; i++) // Спауним 'count' пъти този враг
            {
                Vector3 spawnPosition = transform.position + new Vector3(i, 0, 0) * 2;
                if (i % 2 == 0)
                {
                    spawnPosition += new Vector3(0, 0, 2);
                }
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Изчаква определеното време преди да спауни следващия враг
                yield return new WaitForSeconds(spawningCooldown);
            
        }
    }
}
