using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class WaveSystem : MonoBehaviour
{
    [SerializeField] GameObject goblin;
    [SerializeField] GameObject skeleton;
    [SerializeField] GameObject troll;
    [SerializeField] int skeletonSpawnChance;
    [SerializeField] int goblinSpawnChance;
    [SerializeField] int trollSpawnChance;
    private List<GameObject> spawnEnemy; //list for spawning enemies

    private GameManager gameManager;
    private int waveNum = 1;
    private int enemiesCount=0;
    private int deadEnemies = 0;


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        spawnEnemy = new List<GameObject>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    void Start()
    {
        howManyEnemiesToSpawn(4);
    }

    // Update is called once per frame
    void Update()
    {
       enemiesCount = spawnEnemy.Count;
    }
   
    public void enemyDeath()
    {
        deadEnemies++;
        Debug.Log("Dead enemies: " + deadEnemies);

        if (deadEnemies == enemiesCount)
        {
            waveNum++;
            Debug.Log("wave num " + waveNum);
            Debug.Log("pre wave");
            gameManager.setGameState(GameState.PreWave);
            deadEnemies = 0;
       
        }
    }
    public void chooseRandomEnemy()
    {
        int spawnChance = goblinSpawnChance + skeletonSpawnChance + trollSpawnChance + 1;
        int rand = UnityEngine.Random.Range(1, spawnChance);
        
        if (rand <= goblinSpawnChance)
        {
            spawnEnemy.Add(goblin);
        }
        else if (rand <= skeletonSpawnChance)
        {
            spawnEnemy.Add(skeleton);
        }
        else { 
            spawnEnemy.Add(troll);
        }
    }
    public void howManyEnemiesToSpawn(int count)
    {
        for(int i = 0; i < count; i++)
        {
            chooseRandomEnemy();
        }
    }

    public void spawnWaves()
    {
        StartCoroutine(spawnEnemiesCoroutine(spawnEnemy, 2f));
    }
    public IEnumerator spawnEnemiesCoroutine(List<GameObject> enemy, float spawningCooldown)
    {
       
        for (int i = 0; i < enemy.Count; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(8 * i, 0, 10);
            if (i % 2 == 0)
            {
                spawnPosition += new Vector3(10, 0, 8 * i);
            }

            Instantiate(enemy[i], spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawningCooldown);

        }
    }

}