using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSorce;//used for playing background music
    [SerializeField] AudioSource sfxSorce;//used for playing sound efects
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip fireballSfx;
    [SerializeField] AudioClip runestoneSfx;
    [SerializeField] AudioClip electricitySfx;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 0)//index 0 is the main menu
        {
            musicSorce.clip = mainMenuMusic;
        }
        else
        {  
            musicSorce.clip = backgroundMusic;
        }
        musicSorce.Play();
    }
  

    public void pauseAllSounds() {
        pauseSFX();
        pauseMusic();
    }
    public void unpauseAllSounds()
    {
        unpauseSFX();
        unpauseMusic();
    }
    
    public void pauseSFX()//pauses music Sorce    
    {
        sfxSorce.Pause();
    }
    public void unpauseSFX()
    {
        sfxSorce.UnPause();
    }
    public void pauseMusic()//pauses music Sorce    
    {
        musicSorce.Pause();  
    }
    public void unpauseMusic()
    {
        musicSorce.UnPause();
    }
    public void playSoundEfects(AudioClip sfx)
    {
        sfxSorce.PlayOneShot(sfx);
    }


  
    //Geters
    public AudioClip getBackgroundMusic()
    {
        return backgroundMusic;
    }

    public AudioClip getMenuMusic()
    {
        return mainMenuMusic;
    }
    public AudioClip getDeathSound()
    {
        return deathSound;
    }
    public AudioClip getFireballSfx()
    {
        return fireballSfx;
    }

    public AudioClip getRunestoneSfx()
    {
        return runestoneSfx;
    }

    public AudioClip getElectricitySfx()
    {
        return electricitySfx;
    }

}
