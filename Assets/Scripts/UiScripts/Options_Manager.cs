using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

public class Options_Manager : MonoBehaviour
{
    public static Options_Manager instance;
    public AudioMixer volumeMixer;
    Resolution[] resArr;//we put all existing resolutions here
    [SerializeField] private TMP_Dropdown resDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    // [SerializeField] private
    [SerializeField] private Toggle fullScreenToggle; 
    [SerializeField] private TMP_Text toggle“ext;    
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;



    private void Update()
    {
        Debug.Log("fullscreen: "+ getFullScreen());
    }
    private void Start()
    { // resulutuonsArr[0] = Screen.resolutions[0];//both are arrays
     
        resArr = Screen.resolutions;
        showQualityStart();
        checkIfFullScreen();
        resDropdown.ClearOptions(); //clears options before adding resolutions
        addResolutions();           // adds resolution to the dropdown witch accepts an array list
        loadPreferences();          //loads player settings
        Debug.Log("loadedPrefrence");
        Screen.fullScreen = true;

    }
 
    private void OnEnable()
    {
        resArr = Screen.resolutions;
        showQualityStart();
        checkIfFullScreen();
        resDropdown.ClearOptions(); //clears options before adding resolutions
        addResolutions();           // adds resolution to the dropdown witch accepts an array list
        loadPreferences();

    }
    //SETTINGS MENU
    public void addResolutions()//ads resolutions
    {
      
      
        System.Array.Reverse(resArr); // reverse the array so we get res from top to bottom
      
        List<string> resOptions = new List<string>();
        int currentResolutionIndex = -1;

        for (int i = 0; i < resArr.Length; i++)
        {
           
            string option = resArr[i].width + " x " + resArr[i].height+ "  hz " + resArr[i].refreshRateRatio;
           
                resOptions.Add(option);
          
             //searches for our resolution
            if (resArr[i].width == Screen.currentResolution.width &&
                resArr[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resDropdown.AddOptions(resOptions);


        // automaticly pics if we dont have a prefrence
        if (!PlayerPrefs.HasKey("resolutionIndex") && currentResolutionIndex != -1)//if player hasn't selected a res and we have found a perfect resolution
        {
            setCurrentRes(currentResolutionIndex);
        }

            PlayerPrefs.Save();//save prefrence after every change
    }


    private void setCurrentRes(int index)
    {
     
        Resolution selected = resArr[index];//we take the selected res from the array and add it
        resDropdown.value = index;
        Screen.SetResolution(selected.width, selected.height, getFullScreen());
        resDropdown.RefreshShownValue();
        PlayerPrefs.SetInt("resolutionIndex", index);
   
    }

    public void OnDropdownResolutionValueChanged(int index)//we choose the prefrence with mouse
    {
        if (index != PlayerPrefs.GetInt("resolutionIndex", -1))
        {
            setCurrentRes(index);//sets res to index in the arr with resolutions
        }
    }



    public void setQuality(int qualityIndex)//after setting a quality it adds to preffrence
    {
        QualitySettings.SetQualityLevel(qualityIndex);//sets quality to unity settings
        PlayerPrefs.SetInt("quality", qualityIndex);//adds the prefrence
        PlayerPrefs.Save();

    }
    private void loadPreferences()//loads prefrences if there are any
    {
        if (PlayerPrefs.HasKey("quality"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));
        }

        loadVolumePref("sfxVolume",sfxSlider);
        loadVolumePref("musicVolume",musicSlider);

        if (PlayerPrefs.HasKey("fullScreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("fullScreen", 1) == 1;//accepts only bool 
            
        } 

        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            setCurrentRes(PlayerPrefs.GetInt("resolutionIndex"));
        }
     
    }
  
    public void loadVolumePref(string volName,Slider volSlider)
    {
        if (PlayerPrefs.HasKey(volName))
        {
            float volume = PlayerPrefs.GetFloat(volName);
            volSlider.value = volume;//sets slider ui to voliume
            float dB = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20;
            volumeMixer.SetFloat(volName, dB);
        }
        else
        {
            volSlider.value = 1f;
            float dB = Mathf.Log10(1f) * 20;
            volumeMixer.SetFloat(volName, dB);
        }

    }
    public void setSfxVolume(float volume)
    {
        setVolume("sfxVolume",volume);
    }
    public void setMusicVolume(float volume)
    {
        setVolume("musicVolume", volume);
    }
    public void setVolume(string volName,float volume)
    {
        float dB = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20;
        volumeMixer.SetFloat(volName, dB);
        PlayerPrefs.SetFloat(volName, volume);//the player pref has the same name as the vol mixer param for now
        PlayerPrefs.Save();
    }
    public void showQualityStart() {//shows quality button ui
        if (PlayerPrefs.HasKey("quality"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("quality");
        }
        else
        {
            qualityDropdown.value = 0;
        }
            qualityDropdown.RefreshShownValue();
    }
    public void setFullScreen(bool isFullScreen) {//,full screen setting and saves pref

        Screen.fullScreen = isFullScreen;
        if (isFullScreen)
        {
            PlayerPrefs.SetInt("fullScreen", 1);//1  fullscren

            toggle“ext.text = "FullScreen On";
        }
        else
        {
            PlayerPrefs.SetInt("fullScreen", 0);//0 minimized
            toggle“ext.text = "FullScreen Off";

        }
        fullScreenToggle.isOn = isFullScreen;// sets toggle ui to the option
      
        PlayerPrefs.Save();

    }
    public void checkIfFullScreen()//changes text on the ui text for full screen
    {
        bool isFullScreen = PlayerPrefs.GetInt("fullScreen", 1) == 1;

        fullScreenToggle.isOn = isFullScreen;
        fullScreenToggle.isOn = Screen.fullScreen;
        if (isFullScreen) { 
            toggle“ext.text = "FullScreen On";
        }
        else
        {
            toggle“ext.text = "FullScreen Off";
        }

    }

    public bool getFullScreen()
    {
        if (!PlayerPrefs.HasKey("fullScreen"))
        {
            return true; 
        }
        return PlayerPrefs.GetInt("fullScreen") == 1;
    }

    //MAIN MENY
    public void exitGame()
    {
        Application.Quit();
    }
    public void playGame()
    {
        SceneManager.LoadScene(1);
    }
}