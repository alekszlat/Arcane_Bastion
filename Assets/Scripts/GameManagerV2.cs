using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public enum GameStateV2 //used for diffrent game states
{
    Paused,
    Gameplay,
    PreWave,
    Death
}

public class GameManagerV2 : MonoBehaviour
{

    [SerializeField] static bool isPaused = false;
    [SerializeField] static GameManager instance;
    [SerializeField] GameStateV2 gameState;
    private WaveSystemV2 waveSystemV2;
    private float Timer;

    public float preWaveTimer = 10f;

    private void Awake()
    {
        Timer = preWaveTimer;
        gameState = GameStateV2.PreWave;
        GameObject waveSystemV2Object = GameObject.FindGameObjectWithTag("WaveSystemV2");
        if (waveSystemV2Object != null)
        {
            waveSystemV2 = waveSystemV2Object.GetComponent<WaveSystemV2>();
            WaveSystemV2.OnWaveStarted += OnWaveStarted; // Subscribe to wave start event
            WaveSystemV2.OnWaveCompleted += OnWaveCompleted; // Subscribe to wave completion event
        }
        else
        {
            Debug.LogError("WaveSystemV2 object not found!");
        }
    }

    private void OnWaveStarted()
    {
        Debug.Log("Wave started. Transitioning to Gameplay.");
        setGameState(GameStateV2.Gameplay);
    }

    private void OnWaveCompleted()
    {
        Debug.Log("Wave completed");
        Debug.LogError("Wave complited");
        setGameState(GameStateV2.PreWave);
    }
    private void OnDestroy()
    {
        WaveSystemV2.OnWaveCompleted -= OnWaveCompleted; // Unsubscribe from event
        WaveSystemV2.OnWaveStarted -= OnWaveStarted;
    }

    public void Update()
    {
        timerStates();
    }

    public void timerStates()
    {
        if (gameState == GameStateV2.PreWave)
        {
            Timer -= Time.deltaTime;
            Debug.Log("PreWave Timer: " + Timer); // Debug the timer value
            if (Timer <= 0f)
            {
                waveSystemV2.spawnWaves();
            }
        }
    }

    public void setGameState(GameStateV2 gameState)
    {
        if (this.gameState == gameState) return;

        this.gameState = gameState;

        if (gameState == GameStateV2.Paused)
        {
            isPaused = true;
            Time.timeScale = 0f;
        }
        else if (gameState == GameStateV2.Gameplay)
        {
            isPaused = false;
            Time.timeScale = 1f;
        }
        else if (gameState == GameStateV2.PreWave)
        {
            Timer = preWaveTimer; // Reset the timer
            Debug.Log("PreWave timer reset to: " + preWaveTimer);
        }
    }
}
