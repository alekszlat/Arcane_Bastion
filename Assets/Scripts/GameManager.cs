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
        //gameState няма .Paused и тк не можем от там да ги accessnem ,трябва от самия енъм и така да правим проверки
        if (gameState == GameState.Paused)
        {
            isPaused = true;
            Time.timeScale = 0f; // Спира времето
        }
        else if (gameState == GameState.Gameplay)
        {
            isPaused = false;
            Time.timeScale = 1f; // Възстановява времето
        }

    }
    public GameState getGameState()
    {
        return gameState;
    }

}
