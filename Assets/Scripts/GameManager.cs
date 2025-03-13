using UnityEditor.Playables;
using UnityEngine;

public enum GameState
{
    Paused,
    Gameplay,
    PreWave,
    Death
}
public class GameManager : MonoBehaviour
{
    public static bool isPaused = false;
    public static GameManager instance;
    public GameState gameState;
    private WaveSystem waveSystem;
    
    private float preWaveTimer = 10f;
    private void Awake()
    {
        gameState = GameState.PreWave;
        waveSystem = GameObject.FindGameObjectWithTag("WaveSystem").GetComponent<WaveSystem>();
              
       
    }
    public void Update()
    {
        timerStates();
        Debug.Log(gameState);

    }

    public void timerStates()
    {
        if (gameState == GameState.PreWave)
        {
            preWaveTimer -= Time.deltaTime;
            if (preWaveTimer <= 0f)
            {
                waveSystem.spawnWaves();
                setGameState(GameState.Gameplay);
            }
        }

    }
    public void setGameState(GameState gameState)
    {
        if (this.gameState == gameState) return;

        this.gameState = gameState;
      
        if (gameState == GameState.Paused)
        {
            isPaused = true;
            Time.timeScale = 0f; // stops time,pauses game for time.deltaTime scripts
        }
        else if (gameState == GameState.Gameplay)
        {
            isPaused = false;
            Time.timeScale = 1f; // unpauses
        }else if (gameState == GameState.PreWave)
        {
           preWaveTimer = 20f;
        }
        

    }
   
public GameState getGameState()
    {
        return gameState;
    }

}

/* public void waveTimerLogic(GameState gameState,ref float timer,float preWaveTimer = 20f)
 {

     timer -= Time.deltaTime;
     if (timer <= 0f)
     {
         this.gameState = gameState;
     }
 }*/