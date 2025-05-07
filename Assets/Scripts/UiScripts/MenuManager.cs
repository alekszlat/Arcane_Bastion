using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject pauseScreen;
    private GameManagerV2 gameManagerV2;
    private GameStateV2 temp;

    private void OnValidate()
    {
        if (gameManagerV2 == null)
        {
            gameManagerV2 = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerV2>();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pauseGame();

    }
   
    public void restartGame()
    {
        gameManagerV2.setGameState(GameStateV2.PreWave);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      
    }
   
    public void showDeathScreen()
    {
        gameManagerV2.setGameState(GameStateV2.Death);
     
        deathScreen.SetActive(true);
    }
    public void pauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
        pauseScreen.SetActive(true);
        temp = gameManagerV2.getGameState();
        gameManagerV2.setGameState(GameStateV2.Paused);
    }
}
    public void unpauseGame()
    {
        if (temp == GameStateV2.Paused)
        {
            temp = GameStateV2.PreWave;
        }
        pauseScreen.SetActive(false);
        gameManagerV2.setGameState(temp);
    }
    public void goToMenu()
    {
        SceneManager.LoadScene(0);
    }  

}
