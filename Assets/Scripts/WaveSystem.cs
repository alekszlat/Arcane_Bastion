using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
enum waveSpawnerType
{
    GoblinSpawner,
    SkeletonSpawner,
    TrollSpawner,
    MixedSpawner
}

public class WaveSystem : MonoBehaviour
{
    private GameManager gameManager;
    private int sumOfEnemiesCount;

    [SerializeField] GameObject goblin;
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject troll;
    public static WaveSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
      
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

   
    public void spawnWaves()
    {

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
