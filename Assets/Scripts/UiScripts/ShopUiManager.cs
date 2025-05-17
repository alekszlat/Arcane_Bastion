using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum ShopAnimation //used for diffrent game states
{
   AnimationCompeled,NotActive
}


public class ShopUiManager : MonoBehaviour
{
    [SerializeField] GameObject shop;//MainShopContainer that has everything in it
    [SerializeField] Button fireBallUpgradeButon;
    [SerializeField] Button electricityUnlockButton;
    [SerializeField] Button runestoneUnlockButton;
    [SerializeField] Button electricityUpgradeButton;
    [SerializeField] Button runestoneUpgradeButton;
    [SerializeField] TextMeshProUGUI currentMoneyCount;
    [SerializeField] TextMeshProUGUI upgradeFireBallCostText;
    [SerializeField] TextMeshProUGUI upgradeRunestoneCostText;
    [SerializeField] TextMeshProUGUI unlockRunestoneCostText;
    [SerializeField] TextMeshProUGUI upgradeElectricityCostText;
    [SerializeField] TextMeshProUGUI unlockElectricityCostText;
    [SerializeField] GameObject shopIndicator;
    Vector3 shopIndicatorStartPos;
    Vector3 shopStartPos;
    ShopAnimation shopAnimation=ShopAnimation.NotActive;
    ShopAnimation shopIndicatorAnimation = ShopAnimation.NotActive;
    
    private bool canOpenShop = true;//checks if shop can be opened
    private bool canCloseShop = false;//checks if shop can be closed
 
    public static bool shopIsOpen = false;//checks if shop is opened
    private GameManagerV2 gameManagerV2;
    private PlayerController playerController;
    private Abilities fireBallSkill;
    private Abilities electricitySkill;
    private Abilities runestoneSkill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnValidate()
    {
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        }
        if (gameManagerV2 == null)
        {
            gameManagerV2 = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManagerV2>();
        }
    }

    // Update is called once per frame
    private void Start()
    {
        shopIsOpen = false;
        shopIndicatorStartPos = shopIndicator.transform.position;
        shopStartPos = shop.gameObject.transform.position;

    }
    void Update()
    {
        fireBallSkill = playerController.getFireBallAbility();
        electricitySkill = playerController.getElectricityAbility();
        runestoneSkill = playerController.getRunestoneAbility();
        isShopOpen();
        showMoneyCountText();
        setAbilitiesText();
        showShopIndicator();
        checkIfPlayerCanPurchaseAnyAbility();


    }
    
    public void showMoneyCountText()
    {
        currentMoneyCount.text = "Money: "+playerController.getPlayerMoney().ToString();
    }
     
    public void isShopOpen()
    {
     
        if (gameManagerV2.getGameState() == GameStateV2.Death || gameManagerV2.getGameState() == GameStateV2.Paused)
        {
            return;                                              //this prevent cursor remaining locked after player death
        }
        

        if (gameManagerV2.getGameState() != GameStateV2.PreWave) //If prewave ends exit shop
        {
            shopAnimation = ShopAnimation.NotActive;
            
            closeShop();
            return;
        }
        if (!shopIsOpen && Input.GetKeyDown(KeyCode.P))//if shop isn't open you can acces shop by pressing P
        {
             shopAnimation = ShopAnimation.NotActive;
     
            openShop();
        }
        else if (shopIsOpen && Input.GetKeyDown(KeyCode.P))//if shop is open you can exit shop by pressing P
        {
            shopAnimation = ShopAnimation.NotActive;
      
            closeShop();
        }

    }

  public void openShop()
    {
        if (!canOpenShop) return; //if shop cant be open return
        shop.gameObject.SetActive(true);           //shows shop
        if (shopAnimation== ShopAnimation.NotActive) {//if animation up has not happend yet 
            shop.gameObject.transform.DOMoveY(shopStartPos.y + 850, 1.4f).OnComplete((TweenCallback)(() => {//after animation ends
                canCloseShop = true;//we can now close the shop     
                shopAnimation = ShopAnimation.AnimationCompeled;
            }));
            
           
        }
        shopIsOpen = true;                         //indicates shop is open
        Cursor.lockState = CursorLockMode.None;    //unlocks cursor
        Cursor.visible = true;                     //cursor is visable
    }
    public void closeShop()
    {
        if (!canCloseShop) return;//if shop cant be closed return
        canOpenShop = false;//while closing the shop we cant open it for a while
        if (shopAnimation == ShopAnimation.NotActive) { //if hasn't happend once it happends once only
            shop.gameObject.transform.DOMoveY(shopStartPos.y - 1000, 1).OnComplete((TweenCallback)(() => //On complete is like a couroutine in this case waits for a second
        {
            canOpenShop = true;//after completion shop can be opened now
            shop.gameObject.transform.position = shopStartPos;//resets shop to starting position
            shop.gameObject.SetActive(false);          //hides shop

           
            shopAnimation = ShopAnimation.AnimationCompeled; //makes sure animation happends once
        }));
            //are outside so it defenetly happends
            Cursor.lockState = CursorLockMode.Locked;    //locks cursor
            Cursor.visible = false;                      //makes cursor invisable
            shopIsOpen = false;                        //indicates shop is closed
        }
    }


    public void showShopIndicator()
    {
        GameStateV2 currentGameState = gameManagerV2.getGameState();
        if (currentGameState != GameStateV2.PreWave && currentGameState!=GameStateV2.Paused || shopIsOpen)//DISABLES INDICATOR
        {
            shopIndicator.transform.position = shopIndicatorStartPos;//return shop to start
            shopIndicator.SetActive(false);
            shopIndicatorAnimation = ShopAnimation.NotActive;
        }
        else {//ACTIVATES INDICATOR
           
            shopIndicator.SetActive(true);
            if (shopIndicatorAnimation==ShopAnimation.NotActive)//checks if animation has happened already
            {
                shopIndicator.transform.DOMoveX(shopIndicatorStartPos.x - 300, 1f);//moves indicator
                shopIndicatorAnimation = ShopAnimation.AnimationCompeled;
            }
        }
       
    }
   
    
    //ON CLICK UPGRADES OR UNLOCKES AN ABILITY AND HIDES BUTTON ON PURCHASE

    public void checkIfPlayerCanPurchaseAnyAbility()
    {
        checkIfPlayerCanUpgradeAnAbility(fireBallSkill, fireBallUpgradeButon);
        checkIfPlayerCanUnlockAnAbility(electricitySkill, electricityUnlockButton);
        checkIfPlayerCanUpgradeAnAbility(electricitySkill, electricityUpgradeButton);
        checkIfPlayerCanUnlockAnAbility(runestoneSkill, runestoneUnlockButton);
        checkIfPlayerCanUpgradeAnAbility(runestoneSkill, runestoneUpgradeButton);
    }

    public void checkIfPlayerCanUpgradeAnAbility(Abilities abilities, Button button)
    {
        
        if (playerController.notEnoughMoneyToUpgradeAnAbility(abilities))
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
    public void checkIfPlayerCanUnlockAnAbility(Abilities abilities,Button button)
    {
        //50 leva moga da kupq ,70 upgrade  
        if (playerController.notEnoughMoneyToUnlockAnAbility(abilities))
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
    public void upgradeFireBall()
    {
        playerController.upgradeAbility(fireBallSkill);
        fireBallUpgradeButon.gameObject.SetActive(false);
    }

    public void unlockRunestone()
    {
        playerController.unlockAbility(runestoneSkill);
        runestoneUnlockButton.gameObject.SetActive(false);
        runestoneUpgradeButton.gameObject.SetActive(true);
    }

    public void upgradeRunestone()
    {
        playerController.upgradeAbility(runestoneSkill);
        runestoneUpgradeButton.gameObject.SetActive(false);
    }

    public void unlockElectricity()
    {
        playerController.unlockAbility(electricitySkill);
        electricityUnlockButton.gameObject.SetActive(false);
        electricityUpgradeButton.gameObject.SetActive(true);
    }

    public void upgradeElectricity()
    {
        playerController.upgradeAbility(electricitySkill);
        electricityUpgradeButton.gameObject.SetActive(false);
    }
    public void setAbilitiesText()
    {
        upgradeFireBallCostText.text = "Upgrade Fireball for "+fireBallSkill.getAbilityUpgradeCost()+" Coins";
        upgradeRunestoneCostText.text = "Upgrade Runestone for " + runestoneSkill.getAbilityUpgradeCost() + " Coins";
        unlockRunestoneCostText.text = "Unlock Runestone for " + runestoneSkill.getAbilityUnlockCost() + " Coins";
        upgradeElectricityCostText.text = "Upgrade Electricity for " + electricitySkill.getAbilityUpgradeCost() + " Coins";
        unlockElectricityCostText.text = "Unlock Electricity for " + electricitySkill.getAbilityUnlockCost() + " Coins";
    }
  
}
