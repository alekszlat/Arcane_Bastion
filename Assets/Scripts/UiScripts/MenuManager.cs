using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] CanvasGroup deathScreenGroup;
    [SerializeField] CanvasGroup pauseScreenGroup;

    private GameManagerV2 gameManagerV2;
    private GameStateV2 temp;
    private float tempTime;
    bool canUnpause = false;//checks if game can be unpaused
    bool canPause = true;// checks if game can be paused

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
        deathScreenGroup.gameObject.SetActive(true);
        deathScreenGroup.DOFade(1,2);
    }
    public void pauseGame()
    {
      
        if (Input.GetKeyDown(KeyCode.Escape) && gameManagerV2.getGameState() != GameStateV2.Paused) {
            if (!canPause) return;
            canPause = false;
            canUnpause = false;
            pauseScreenGroup.alpha = 0f; // pausescreen is visable
            tempTime = gameManagerV2.getTimerUntilPrewaveEnds(); //gets wave time before game pauses
            temp = gameManagerV2.getGameState();//gets gamestate before game pauses
            pauseScreenGroup.gameObject.SetActive(true);//pause screen is now visable

            pauseScreenGroup.DOFade(1, 0.5f).OnComplete(() =>//after fading game pauses
            {
                gameManagerV2.setGameState(GameStateV2.Paused);
                canUnpause = true;
            }); 
            
          
     }else if(Input.GetKeyDown(KeyCode.Escape) && gameManagerV2.getGameState() == GameStateV2.Paused)
        {
            unpauseGame();
        }
}
    public void unpauseGame()//there are 2 ways to acces upause game by pressing a button or  espace
    {
        if (!canUnpause) return;
        canPause = false;
        canUnpause = false;
        pauseScreenGroup.DOFade(0, 1f).OnComplete(() =>
        {
          pauseScreenGroup.gameObject.SetActive(false);
          canPause = true;

        });
         
        gameManagerV2.setIsTimerPaused(true);//makes sure timer doesnt reset after pause
        gameManagerV2.setGameState(temp);
   
        gameManagerV2.setTimerUntilPrewaveEnds(tempTime);
        
        Cursor.lockState = CursorLockMode.Locked;    //locks cursor
        Cursor.visible = false;                     //cursor is invisable
    }
    public void goToMenu()
    {
        SceneManager.LoadScene(0);
    }  
  
}
