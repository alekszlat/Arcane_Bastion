using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;

public enum GameStateV2 //used for diffrent game states
{
    Paused,
    Gameplay,
    PreWave,
    Death
}

public class GameManagerV2 : MonoBehaviour
{
    private GameStateV2 gameState;
    private WaveSystemV2 waveSystemV2;
    private PlayerController playerController;
    private float Timer;
    private bool isTimerPaused = false;//if true timer continiues,if false timer resets 

    public float preWaveTimer = 10f;

    private void Start()
    {
      
        if (Time.timeScale == 0)//if for some reason TimeScale is 0 return time to normal
        {
            Time.timeScale = 1;
        }
    }
    private void Awake() { 
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        //Debug.Log("Wave started. Transitioning to Gameplay.");
        setGameState(GameStateV2.Gameplay);
    }

    private void OnWaveCompleted()
    {
        //Debug.LogError("Wave complited");
        setGameState(GameStateV2.PreWave);
    }
    private void OnDestroy()
    {
        WaveSystemV2.OnWaveCompleted -= OnWaveCompleted; // Unsubscribe from event
        WaveSystemV2.OnWaveStarted -= OnWaveStarted;
    }

    public void Update()
    {
     
        Debug.Log("Is shop open: " + ShopUiManager.shopIsOpen);

        if (gameState == GameStateV2.PreWave)
        {
            if (ShopUiManager.shopIsOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    timerStates();
    
    }

    public void timerStates()
    {
       
        if (gameState == GameStateV2.PreWave)
        {
            Timer -= Time.deltaTime;
            //Debug.Log("PreWave Timer: " + Timer); // Debug the timer value
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
            Cursor.lockState = CursorLockMode.None;    //unlocks cursor
            Cursor.visible = true;                     //cursor is visable
        
            Time.timeScale = 0f;
        }
        else if (gameState == GameStateV2.Gameplay)
        {
            Cursor.lockState = CursorLockMode.Locked;    //locks cursor
            Cursor.visible = false;                     //cursor is invsible
           
            Time.timeScale = 1f;
        }

        else if (gameState == GameStateV2.PreWave)
        {     
         
            if (!isTimerPaused)//if timer isnt paused the game resets( After Waves), else if its paused timer continiues
            {
                Timer = preWaveTimer; // Reset the timer 
            }
            isTimerPaused = false;
            playerController.setPlayerMana(100);
            Time.timeScale = 1f;
       
        }
        else if(gameState == GameStateV2.Death)
        {
            Cursor.lockState = CursorLockMode.None;    //unlocks cursor
            Cursor.visible = true;                     //cursor is visable

            Time.timeScale = 0f;
        }
    }
    public GameStateV2 getGameState() {
        return gameState;
    }
    public float getTimerUntilPrewaveEnds()
    {
        return Timer;
    }

    public void setTimerUntilPrewaveEnds(float Timer)
    {
        this.Timer = Timer;
    }
    public void setIsTimerPaused(bool isTimerPaused)
    {
        this.isTimerPaused = isTimerPaused;
    }
    public bool getIsTimerPaused()
    {
        return isTimerPaused;
    }
}
