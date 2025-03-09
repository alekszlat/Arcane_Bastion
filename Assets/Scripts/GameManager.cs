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
        //gameState ���� .Paused � �� �� ����� �� ��� �� �� accessnem ,������ �� ����� ���� � ���� �� ������ ��������
        if (gameState == GameState.Paused)
        {
            isPaused = true;
            Time.timeScale = 0f; // ����� �������
        }
        else if (gameState == GameState.Gameplay)
        {
            isPaused = false;
            Time.timeScale = 1f; // ������������ �������
        }

    }
    public GameState getGameState()
    {
        return gameState;
    }

}
