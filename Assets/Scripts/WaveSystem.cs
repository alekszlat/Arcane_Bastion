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
        GameState gameState = gameManager.getGameState();
        if (gameState == GameState.SpawningWaves)
        {
          //  StartCoroutine(spawnEnemiesCoroutine(go, enemyType, spawningCooldown));


        }
    }
    public IEnumerator spawnEnemiesCoroutine(GameObject enemyPrefab,int enemyCount, float spawningCooldown)
    {
            for (int i = 0; i < enemyCount; i++) // ������� 'count' ���� ���� ����
            {
                Vector3 spawnPosition = transform.position + new Vector3(i, 0, 0) * 2;
                if (i % 2 == 0)
                {
                    spawnPosition += new Vector3(0, 0, 2);
                }
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // ������� ������������ ����� ����� �� ������ ��������� ����
                yield return new WaitForSeconds(spawningCooldown);
            
        }
    }
}
