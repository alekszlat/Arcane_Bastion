using UnityEngine;

public enum GameState
{
    Paused,
    Gameplay,
    SpawningWaves,
    PreWave,
    Death
}
public class GameManager : MonoBehaviour
{
    public static bool isPaused = false;
    public static GameManager instance;
    public GameState gameState;
    private void Awake()
    {
        gameState = GameState.Gameplay;
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

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
        }
        if (gameState == GameState.SpawningWaves)//когато и
        {
         //  WaveSystem.instance.StartWaveSpawning();
        }

    }
   
public GameState getGameState()
    {
        return gameState;
    }

}
