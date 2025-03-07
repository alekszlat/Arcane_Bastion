using UnityEngine;
using UnityEngine.UI;
public class PlayerUiManager : MonoBehaviour
{
    public Image fireBallSkillBackground;
    public Image crystalSkillBackground;
    public Image electricitySkillBackground;
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
 
      float fireBallCooldown= PlayerController.fireBallCooldown.timer;
      float fireBallTimer = PlayerController.fireBallCooldown.cooldownTime;
      abilityUiCooldown(fireBallCooldown, fireBallTimer, fireBallSkillBackground,fireBallTimer*3);
     

    }
    private void abilityUiCooldown(float maxCooldown, float currentCooldown,Image abilityImage,float animationSpeed)
    { //update cooldwoln

        float fillCooldown = Mathf.Clamp01(maxCooldown/currentCooldown);
      

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
