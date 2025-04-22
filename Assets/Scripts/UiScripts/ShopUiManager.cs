using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUiManager : MonoBehaviour
{
    [SerializeField] GameObject shop;
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
    public static bool shopIsOpen = false;
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
    void Update()
    {
        fireBallSkill = playerController.getFireBallAbility();
        electricitySkill = playerController.getElectricityAbility();
        runestoneSkill = playerController.getRunestoneAbility();
        isShopOpen();
        showMoneyCountText();
        setAbilitiesText();
    }
    public void showMoneyCountText()
    {
        currentMoneyCount.text = "Money: "+playerController.getPlayerMoney().ToString();
    }
  
    public void isShopOpen()
    {
        if (gameManagerV2.getGameState() == GameStateV2.Death|| gameManagerV2.getGameState() == GameStateV2.Paused)
        {
            return;                                              //this prevent cursor remaining locked after player death
        }
        if (gameManagerV2.getGameState() != GameStateV2.PreWave) //If prewave ends exit shop
        {
            closeShop();
            return;
        }
        if (!shopIsOpen && Input.GetKeyDown(KeyCode.P))//if shop isn't open you can acces shop by pressing P
        {
            openShop();
        }
        else if (shopIsOpen && Input.GetKeyDown(KeyCode.P))//if shop is open you can exit shop by pressing P
        {
            closeShop();
        }

    }
    public void openShop()
    {
        shop.gameObject.SetActive(true);           //shows shop
        shopIsOpen = true;                         //indicates shop is open
        Cursor.lockState = CursorLockMode.None;    //unlocks cursor
        Cursor.visible = true;                     //cursor is visable
    }
    public void closeShop()
    {
        shop.gameObject.SetActive(false);          //hides shop
        shopIsOpen = false;                        //indicates shop is closed
        Cursor.lockState = CursorLockMode.Locked;  //locks cursor
        Cursor.visible = false;                    //makes cursor invisable
    }

    //ON CLICK UPGRADES OR UNLOCKES AN ABILITY AND HIDES BUTTON ON PURCHASE
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
