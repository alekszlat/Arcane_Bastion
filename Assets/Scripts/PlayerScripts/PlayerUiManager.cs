using UnityEngine;
using UnityEngine.UI;
public class PlayerUiManager : MonoBehaviour
{
    
    [SerializeField] Image fireBallSkillBackground;
    [SerializeField] Image crystalSkillBackground;
    [SerializeField] Image electricitySkillBackground;
    private PlayerController playerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        areAbilitiesCooldownUi();
    }
    public void areAbilitiesCooldownUi()
    {
         
       
      
       abilitiesAnimation(playerController.getFireBallAbility(), fireBallSkillBackground);
        abilitiesAnimation(playerController.getElectricityAbility(), electricitySkillBackground);
        abilitiesAnimation(playerController.getCristalAbility(), crystalSkillBackground);
    }
    
    private void abilityUiCooldown(float curentCooldownTimer, float maxCooldown, Image abilityImage,float animationSpeed)
    { //update cooldwoln

        float fillCooldown = Mathf.Clamp01(curentCooldownTimer / maxCooldown);
      

        if (fillCooldown > 1)
        {
            abilityImage.fillAmount = 1;
        }
        else
        {
            abilityImage.fillAmount = Mathf.MoveTowards(abilityImage.fillAmount, fillCooldown, animationSpeed * Time.deltaTime);
        }
        

    }
    public void abilitiesAnimation(Abilities ability,Image image)//refactoring code
    {
        float abilityTimer = ability.timer;
        float abilityCooldown = ability.cooldownTime;

        abilityUiCooldown(abilityTimer, abilityCooldown, image, abilityCooldown * 3);
    }
}
