
using UnityEngine;

public enum GameState //used for diffrent game states
{
    Paused,
    Gameplay,
    PreWave,
    Death
}
public class GameManager : MonoBehaviour
{
    [SerializeField] static bool isPaused = false;
    [SerializeField] static GameManager instance;
    [SerializeField] GameState gameState;
    private WaveSystem waveSystem;

    private float preWaveTimer = 10f;
    private void Awake()
    {
        gameState = GameState.PreWave;
        waveSystem = GameObject.FindGameObjectWithTag("WaveSystem").GetComponent<WaveSystem>();
    }
    public void Update()
    {
        Debug.Log("current gamestate: "+gameState);
        timerStates();
    }

    public void timerStates()//has to be in update because its a timer
    {
        if (gameState == GameState.PreWave)
        {
            preWaveTimer -= Time.deltaTime;
            if (preWaveTimer <= 0f)
            {
                waveSystem.spawnWaves();//spawns waves from wave system when the cooldown ends
                Debug.Log("PreWave timer ended. Calling spawnWaves().");
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
            Time.timeScale = 0f; // stops time,pauses game for funcions using time.delta time and ones in fixedUpdate
        }
        else if (gameState == GameState.Gameplay)
        {
            isPaused = false;
            Time.timeScale = 1f; //normal game speed
        }
        else if (gameState == GameState.PreWave)
        {
            preWaveTimer = 10f;//resets timer when we are in preWave
        }
    }
  
}