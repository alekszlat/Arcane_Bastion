using System.Collections;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Abilities;
public class PlayerUiManager : MonoBehaviour
{
    private Abilities fireBallSkill;
    private Abilities electricitySkill;
    private Abilities runestoneSkill;
    private GameManagerV2 gameManagerV2;
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
    [SerializeField] TextMeshProUGUI fireballManaCostText;
    [SerializeField] TextMeshProUGUI runestoneManaCostText;
    [SerializeField] TextMeshProUGUI electricityManaCostText;
    [SerializeField] TextMeshProUGUI timeUntilNextWaveText;
    [SerializeField] Image lockerRunestoneAbilityImage;
    [SerializeField] Image lockerElectrityAbilityImage;



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
        if (gameManagerV2 == null)
        {
            gameManagerV2 = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManagerV2>();
        }
       
 
    }

    // Update is called once per frame
    void Update()
    {
      
        fireBallSkill =playerController.getFireBallAbility();
        electricitySkill=playerController.getElectricityAbility();
        runestoneSkill=playerController.getRunestoneAbility();
       

        float maxMana = 100;
        float currentMana = playerController.getPlayerMana();
        float maxEnemyCount= waveSystemController.getMaxEnemyCount();
        float currentEnemyCount = waveSystemController.getEnemyCount();
        float maxTowerHealth = towerController.getMaxTowerHealth();
        float towerHealth = towerController.getTowerHealth();
        

        areAbilitiesOnCooldownUi();

        updateBar(maxTowerHealth, towerHealth, towerBar, 4f);
        updateBar(maxMana,currentMana,manaBar,0.80f);
        updateBar(maxEnemyCount,currentEnemyCount, waveBar, 0.30f);

        waveNumberUi();
        howMuchManaDoesAbilityCost();
        displayMessageIfManaIsNotEnough();
        isAbilityLocked(electricitySkill, electricityManaCostText,lockerElectrityAbilityImage);
        isAbilityLocked(runestoneSkill, runestoneManaCostText, lockerRunestoneAbilityImage);

        timeUntilNextWaveUi();

    }
    
   
   //UI Func
    public void isAbilityLocked(Abilities ability, TextMeshProUGUI abilityManaCost,Image lockerimage)//if ability is not locked mana is visable and the locker Icon dissaperars
    {
        if (ability.getAbilityStatus() != AbilityStatus.isLocked) {
            abilityManaCost.gameObject.SetActive(true);
            lockerimage.gameObject.SetActive(false);
        }
    }
    public void howMuchManaDoesAbilityCost()//displays how much mana each ability is
    {
        fireballManaCostText.text = fireBallSkill.getManaCost().ToString(); 
        runestoneManaCostText.text = runestoneSkill.getManaCost().ToString();
        electricityManaCostText.text = electricitySkill.getManaCost().ToString();
    }
    public void areAbilitiesOnCooldownUi()//couldown for enemy ui  abilities
    {
        abilitiesAnimation(fireBallSkill, fireBallSkillBackground);
        abilitiesAnimation(electricitySkill, electricitySkillBackground);
        abilitiesAnimation(runestoneSkill, runestoneSkillBackground);
        
    }
   
    private void updateBar(float maxBarCapacity,float currentBarCapacity,Image BarToUpdate,float uiAnimationSpeed)//logic for bars like mana,health,waveBar,slowly makes the bar decrease
    {
        float bar;
        bar = currentBarCapacity / maxBarCapacity;//we need float because fill amount takes a number between 0 and 1  so we devide currentBarCapacity by maxBarCapacity
        BarToUpdate.fillAmount = Mathf.MoveTowards(BarToUpdate.fillAmount, bar, uiAnimationSpeed * Time.deltaTime); 
    }

    private void abilityUiCooldown(float curentCooldownTimer, float maxCooldown, Image abilityImage,float animationSpeed,Abilities ability)//the logic for the Abilities Ui
    {
        if (Abilities.usingAbility == true||ability.getAbilityStatus() == AbilityStatus.isLocked) { abilityImage.fillAmount = 1; } //if player is using an ability the icon turns black for ability ui
        else { 
            float fillCooldown = Mathf.Clamp01(curentCooldownTimer / maxCooldown);
    
            abilityImage.fillAmount = Mathf.MoveTowards(abilityImage.fillAmount, fillCooldown, animationSpeed * Time.deltaTime);
     }
}

    public void abilitiesAnimation(Abilities ability,Image image)//sets the Ui for abilities
    {
        float abilityTimer = ability.getTimer();
        float abilityCooldown = ability.getCooldownTime();

        abilityUiCooldown(abilityTimer, abilityCooldown, image, abilityCooldown * 3,ability);
    }

    public void waveNumberUi()
    {
        int currentWaveNumber=waveSystemController.getGlobalWaveIndex();
        waveNumText.text = currentWaveNumber.ToString(); //sets the curent wave idex 
    }
   
    IEnumerator showNoManaText()
    {
        notEnoughManaText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);            //duration for no mana warning
        notEnoughManaText.gameObject.SetActive(false);
    }
    public void displayMessageIfManaIsNotEnough() {
        int playerMana = playerController.getPlayerMana();
        int electricityAbilityManaCost= electricitySkill.getManaCost();
        int runestoneAbilityManaCost = runestoneSkill.getManaCost();
        if(playerController.checkIfManaIsNotEnoughAfterPressingAnAbility(playerMana,electricityAbilityManaCost)||
           playerController.checkIfManaIsNotEnoughAfterPressingAnAbility(playerMana,runestoneAbilityManaCost))
        {
            StartCoroutine(showNoManaText()); //if player clicks on an ability while he doesnt have enough mana for it it shows no mana warning
        }
        
    }
    public void timeUntilNextWaveUi() //shows the time until the next wave comes (only when the game state is prewave)
    {
        if (gameManagerV2.getGameState() == GameStateV2.PreWave) {
            timeUntilNextWaveText.gameObject.SetActive(true);
            timeUntilNextWaveText.text = " Wave incoming in " + ((int)gameManagerV2.getGameManagerTimeUntilPrewaveEnds()).ToString()+ " seconds";
        }
        else
        {
            timeUntilNextWaveText.gameObject.SetActive(false);
        }
    }

  
    

}
