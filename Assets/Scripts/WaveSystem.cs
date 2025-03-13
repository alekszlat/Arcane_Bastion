using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemySpawn
{
   private GameObject enemyPrefab;
   private int enemyCount;
    public EnemySpawn(GameObject enemyPrefab,int enemyCount)
    {
        this.enemyPrefab = enemyPrefab;
        this.enemyCount = enemyCount;
    }
    public GameObject getEnemyPrefab()
    {
        return enemyPrefab;
    }
    public int getEnemyCount()
    {
        return enemyCount;
    }
    public void setEnemyCount(int enemyCount)
    {
        this.enemyCount = enemyCount;
    }
}
public class WaveSystem : MonoBehaviour
{
    [SerializeField] GameObject goblin;
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject troll;
    [SerializeField] int skeletonSpawnChance;
    [SerializeField] int goblinSpawnChance;
    [SerializeField] int trollSpawnChance;
    EnemySpawn goblinClass;
    EnemySpawn skeletonClass;
    EnemySpawn trollClass;
    private GameManager gameManager;
    private int waveNum = 1;
    private int enemiesCount;
    private EnemyBehaviour enemyBehaviour;
    private int deadEnemies = 0;


    private void Awake()
    {
     DontDestroyOnLoad(this.gameObject);
 
        goblinClass = new EnemySpawn(goblin, 4);
        skeletonClass = new EnemySpawn(skeleton, 2);
        trollClass = new EnemySpawn(troll, 2);
        enemyBehaviour = goblin.GetComponent<EnemyBehaviour>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
     //
     
    }

    // Update is called once per frame
    void Update()
    {


    }



    public void enemyDeath()
    {
        deadEnemies++;
        Debug.Log("Dead enemies: " + deadEnemies);

        if (deadEnemies == enemiesCount)
        {
            waveNum++;
            Debug.Log("wave num "+waveNum);
            Debug.Log("pre wave");
            gameManager.setGameState(GameState.PreWave);
            deadEnemies = 0;
        }
    }
    public EnemySpawn chooseRandomEnemy()
    {
        int spawnChance = goblinSpawnChance + skeletonSpawnChance + trollSpawnChance+1;
        int rand = UnityEngine.Random.Range(1, spawnChance);

        if (rand <= goblinSpawnChance)
        {
            enemiesCount = goblinClass.getEnemyCount();
            return goblinClass;
        }
        else if (rand <= skeletonSpawnChance)
        {
            enemiesCount = skeletonClass.getEnemyCount();
            return skeletonClass;
        }
        else
        {
            enemiesCount = trollClass.getEnemyCount();
            return trollClass;
        }
    }


    public void spawnWaves()
    {
         StartCoroutine(spawnEnemiesCoroutine(chooseRandomEnemy(),2f));
    }
    public IEnumerator spawnEnemiesCoroutine(EnemySpawn enemy, float spawningCooldown)
    {
            for (int i = 0; i < enemy.getEnemyCount(); i++) 
            {
                Vector3 spawnPosition = transform.position + new Vector3(8*i, 0, 10);
            if (i % 2 == 0)
            {
                spawnPosition += new Vector3(10, 0, 8*i);
            }

            Instantiate(enemy.getEnemyPrefab(), spawnPosition, Quaternion.identity);

               
                yield return new WaitForSeconds(spawningCooldown);
            
        }
    }
   
}
