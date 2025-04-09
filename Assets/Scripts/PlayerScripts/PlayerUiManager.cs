using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUiManager : MonoBehaviour
{
    
    [SerializeField] Image fireBallSkillBackground;
    [SerializeField] Image runestoneSkillBackground;
    [SerializeField] Image electricitySkillBackground;
    [SerializeField] Image manaBackground;
    [SerializeField] TextMeshProUGUI notEnoughManaText;
    private PlayerController playerController;
    private WaveSystemV2 waveSystemController;
    private TowerBehaviour towerController;
    private float mana;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        waveSystemController = GameObject.FindGameObjectWithTag("WaveSystemV2").GetComponent<WaveSystemV2>();
        towerController= GameObject.FindGameObjectWithTag("TowerBase").GetComponent<TowerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        int maxMana = 100;
        int currentMana = playerController.getPlayerMana();
        areAbilitiesOnCooldownUi();
        updateManaBar(maxMana,currentMana);
    }
    public void areAbilitiesOnCooldownUi()//couldown for enemy ui  abilities
    {
        abilitiesAnimation(playerController.getFireBallAbility(), fireBallSkillBackground);
        abilitiesAnimation(playerController.getElectricityAbility(), electricitySkillBackground);
        abilitiesAnimation(playerController.getRunestoneAbility(), runestoneSkillBackground);
    }
    
    private void updateManaBar(int maxBarCapacity,int currentBarCapacity)
    {
        mana = currentBarCapacity / maxBarCapacity;
        manaBackground.fillAmount = Mathf.MoveTowards(manaBackground.fillAmount, mana, 2 * Time.deltaTime);
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
   

}
