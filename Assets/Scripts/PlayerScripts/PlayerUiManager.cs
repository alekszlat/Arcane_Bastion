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

      float fireBallTimer = PlayerController.fireBallSkill.timer;
      float fireBallCooldown = PlayerController.fireBallSkill.cooldownTime;
        float electricityTimer = PlayerController.electricitySkill.timer;
        float electricityCooldown = PlayerController.electricitySkill.cooldownTime;
        float cristalTimer = PlayerController.crystalSkill.timer;
        float cristalCooldown = PlayerController.crystalSkill.cooldownTime;


        abilityUiCooldown(fireBallTimer, fireBallCooldown, fireBallSkillBackground, fireBallCooldown * 3);
        abilityUiCooldown(electricityTimer, electricityCooldown, electricitySkillBackground, electricityCooldown * 3);
        abilityUiCooldown(cristalTimer, cristalCooldown, crystalSkillBackground, cristalCooldown * 3);

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
}
