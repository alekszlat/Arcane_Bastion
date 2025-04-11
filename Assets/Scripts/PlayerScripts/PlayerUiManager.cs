using System.Collections;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] Image fireBallSkillBackground;
    [SerializeField] Image runestoneSkillBackground;
    [SerializeField] Image electricitySkillBackground;
    [SerializeField] Image manaBar;
    [SerializeField] Image waveBar;
    [SerializeField] Image towerBar;
    [SerializeField] PlayerController playerController;
    [SerializeField] WaveSystemV2 waveSystemController;
    [SerializeField] TowerBehaviour towerController;
    [SerializeField] TextMeshProUGUI notEnoughManaText;
    [SerializeField] TextMeshProUGUI waveNumText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    void OnValidate()
    {
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        }

        if (waveSystemController == null)
        { 
            waveSystemController = GameObject.FindGameObjectWithTag("WaveSystemV2")?.GetComponent<WaveSystemV2>(); 
        }

        if (towerController == null)
        {
            towerController = GameObject.FindGameObjectWithTag("TowerBase")?.GetComponent<TowerBehaviour>();
        }
 
    }

    // Update is called once per frame
    void Update()
    {
        float maxMana = 100;
        float currentMana = playerController.getPlayerMana();
        float maxEnemyCount= waveSystemController.getMaxEnemyCount();
        float currentEnemyCount = waveSystemController.getEnemyCount();
        float maxTowerHealth = towerController.getMaxTowerHealth();
        float towerHealth = towerController.getTowerHealth();
        areAbilitiesOnCooldownUi();
        updateBar(maxTowerHealth, towerHealth, towerBar, 0.80f);
        updateBar(maxMana,currentMana,manaBar,0.80f);
        updateBar(maxEnemyCount,currentEnemyCount, waveBar, 0.30f);
        waveNumberUi();
        notEnoughMana();


    }
    public void areAbilitiesOnCooldownUi()//couldown for enemy ui  abilities
    {
        abilitiesAnimation(playerController.getFireBallAbility(), fireBallSkillBackground);
        abilitiesAnimation(playerController.getElectricityAbility(), electricitySkillBackground);
        abilitiesAnimation(playerController.getRunestoneAbility(), runestoneSkillBackground);
        
    }
    
    
    private void updateBar(float maxBarCapacity,float currentBarCapacity,Image BarToUpdate,float uiAnimationSpeed)
    {
        float bar;
        bar = currentBarCapacity / maxBarCapacity;
        BarToUpdate.fillAmount = Mathf.MoveTowards(BarToUpdate.fillAmount, bar, uiAnimationSpeed * Time.deltaTime);
    }
    private void abilityUiCooldown(float curentCooldownTimer, float maxCooldown, Image abilityImage,float animationSpeed)//the logic for the ui animation
    {
        if (Abilities.usingAbility == true) { abilityImage.fillAmount = 1; } //if player is using an ability the icon turns black for ability ui
        else { 
            float fillCooldown = Mathf.Clamp01(curentCooldownTimer / maxCooldown);
    
            abilityImage.fillAmount = Mathf.MoveTowards(abilityImage.fillAmount, fillCooldown, animationSpeed * Time.deltaTime);
     }
}
    public void abilitiesAnimation(Abilities ability,Image image)//sets the Ui for abilities
    {
        float abilityTimer = ability.getTimer();
        float abilityCooldown = ability.getCooldownTime();

        abilityUiCooldown(abilityTimer, abilityCooldown, image, abilityCooldown * 3);
    }
    public void waveNumberUi()
    {
        int currentWaveNumber=waveSystemController.getCurrentWaveIndex();
        waveNumText.text = (currentWaveNumber+1).ToString(); //sets the curent wave idex 
    }
   
    IEnumerator showNoManaText()
    {
        notEnoughManaText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        notEnoughManaText.gameObject.SetActive(false);
    }
    public void notEnoughMana() {
        int playerMana = playerController.getPlayerMana();
        int electricityAbilityManaCost= playerController.getElectricityAbility().getManaCost();
        int runestoneAbilityManaCost = playerController.getRunestoneAbility().getManaCost();
        if(!playerController.checkIfManaIsEnough(playerMana,electricityAbilityManaCost)||!playerController.checkIfManaIsEnough(playerMana, runestoneAbilityManaCost))
        {
            StartCoroutine(showNoManaText());
        }
        
    }
    

}
