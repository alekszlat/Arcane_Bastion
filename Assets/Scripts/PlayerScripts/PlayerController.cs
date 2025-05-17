using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Abilities;


public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f; // Force applied when jumping
    [SerializeField] float raycastDistance = 1.1f; // Distance to check for ground

    [Header("Fireball Ability")]
    [SerializeField] private GameObject fireballPrefab;  // Assign your ball prefab in the inspector
    [SerializeField] private Transform castPoint;   // Where the ball spawns
    [SerializeField] float fireballMaxCastDistance = 100f;
    [SerializeField] LineRenderer aimLine;
    [SerializeField] float aimLineDuration = 0.1f;

    [Header("Lightning Ability")]
    [SerializeField] private GameObject lightningPrefab; // Assign your ball prefab in the inspector
    [SerializeField] private GameObject lightningIndicatorPrefab; // Assign your ball prefab in the inspector
    [SerializeField] float lightningMaxCastDistance = 100f;

    [Header("Runestone Ability")]
    [SerializeField] private GameObject runestonePrefab;
    [SerializeField] private GameObject runestoneIndicatorPrefab;
    [SerializeField] float runestoneMaxCastDistance = 100f;
    private Transform TowerPos;

    [Header("Player Abilities")]
    Abilities fireBallSkill = new Abilities(true, 2, 0, Abilities.AbilityStatus.isUnlocked, 0, 50);//object for fireball ability
    Abilities electricitySkill = new Abilities(true, 6, 25, Abilities.AbilityStatus.isLocked, 40, 70);//cooldown check,timer,mana cost,ability status,unlock cost,upgrade cost
    Abilities runestoneSkill = new Abilities(true, 15, 35, Abilities.AbilityStatus.isLocked, 50, 100);
    private int playerMoney = 50;

    [SerializeField] LayerMask aimLayerMask;
    private GameManagerV2 gameManager;

    public static bool usingIndicator;
    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rb;
    private Animator anim;
    private Camera playerCamera;
    private int playerMana = 100;

    private Vector3 raycastOffset = new Vector3(0, 1.1f, 0); // Offset for the raycast origin

    void Start()
    {
        
        fireBallSkill.setAbilityStatus(AbilityStatus.isUnlocked);
        electricitySkill.setAbilityStatus(AbilityStatus.isLocked);
        runestoneSkill.setAbilityStatus(AbilityStatus.isLocked);

        TowerPos = GameObject.FindGameObjectWithTag("Target").transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerV2>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        aimLayerMask = ~(1 << LayerMask.NameToLayer("InvisibleWall"));
        usingIndicator = false;
    }

    // Update is called once per frame
    void Update()
    {


        //ability cooldown is always in update because it activates only when,it's used
        abilityCooldownTimer(fireBallSkill);
        abilityCooldownTimer(electricitySkill);
        abilityCooldownTimer(runestoneSkill);

        // if (ShopUiManager.shopIsOpen) return; //if the shop is open player can't move
        playerMovement();
        playerAbilities();


        Debug.Log("using indicator: " + usingIndicator);

    }

    // PLAYER MOVEMENT
    public void playerMovement()//TRY  put animations in a difrent class (not MonoBehaviour)
    {

        // Create a 2D vector with movement directions.
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Use the vector instead of separate GetKey to get the movement direction to reduce calls for better performance.
        bool isWalkingForward = moveDirection.y > 0;
        bool isWalkingBackward = moveDirection.y < 0;
        bool isWalkingRight = moveDirection.x > 0;
        bool isWalkingLeft = moveDirection.x < 0;

        transform.Translate(Vector3.forward * speed * Time.deltaTime * moveDirection.y);

        anim.SetBool("isWalkingForward", isWalkingForward);
        anim.SetBool("isWalkingBack", isWalkingBackward);

        transform.Translate(Vector3.right * speed * Time.deltaTime * moveDirection.x);
        anim.SetBool("isWalkingRight", isWalkingRight);
        anim.SetBool("isWalkingLeft", isWalkingLeft);

        // Check for jump input
        // Hristo: A trigger could be used instead of a bool.
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            anim.SetBool("isJumping", true);
            Jump();
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
    }

    bool IsGrounded()
    {
        // Cast a ray downward to check for ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position + raycastOffset, Vector3.down, out hit, raycastDistance))
        {
            return true; // Grounded
        }
        return false; // Not grounded
    }

    void Jump()
    {

        // Apply upward force to the Rigidbody
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    // ABILITY MANAGEMENT
    IEnumerator ResetFireballAnimationAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isFireBallCasting", false);
    }

    public void playerAbilities()
    {
        if (Abilities.usingAbility == true || ShopUiManager.shopIsOpen) return; // Prevents casting any abilities if one is already in use

        // Fireball ability
        if (Input.GetKeyDown(KeyCode.Mouse0) && fireBallSkill.getCanUseAbility() && checkIfManaIsEnough(playerMana, fireBallSkill.getManaCost()))
        {
            playerMana -= fireBallSkill.getManaCost();

            fireBallSkill.setCanUseAbility(false);
            anim.SetBool("isFireBallCasting", true);
            fireBallSkill.setTimer(fireBallSkill.getCooldownTime());

           //resets fireball Animation 
            StartCoroutine(ResetFireballAnimationAfterDelay());//stops animation after cast
        }
        else if (!Input.GetKeyDown(KeyCode.Mouse0)) //if mouse1 isnt pressed
        {
            anim.SetBool("isFireBallCasting", false);
        }

        
        // Lightning ability (In progress)
        if (Input.GetKeyDown(KeyCode.Q) && electricitySkill.getCanUseAbility() && checkIfManaIsEnough(playerMana, runestoneSkill.getManaCost()))
        {
            if (isManaDepleted() == true || electricitySkill.getAbilityStatus() == AbilityStatus.isLocked) return;//check if you have enough mana or have unlocked the ability
            StartCoroutine(lightningAbilityMechanic()); // Start the lightning ability process
            electricitySkill.setCanUseAbility(false); // Prevents reusing ability until cooldown
        }

        // Runestone ability
        if (Input.GetKeyDown(KeyCode.E) && runestoneSkill.getCanUseAbility() && checkIfManaIsEnough(playerMana, electricitySkill.getManaCost()))
        {
            if (isManaDepleted() == true|| runestoneSkill.getAbilityStatus() == AbilityStatus.isLocked) return;
            StartCoroutine(runestoneAbilityMechanic()); // Start the runestone ability process
            runestoneSkill.setCanUseAbility(false); // Prevents reusing ability until cooldown
        }
    }
   
    // ABILITY METHODS

    // Called from Animation Event at the release point
    public void ReleaseFireball()
    {
     
        // Calculate direction from camera center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, fireballMaxCastDistance, aimLayerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(fireballMaxCastDistance);
        }

        // Debugging with LineRenderer
        if (aimLine != null)
        {
            aimLine.SetPosition(0, castPoint.position);
            aimLine.SetPosition(1, targetPoint);
            StartCoroutine(ClearAimLine());
        }

        // Calculate direction with slight upward arc
        Vector3 direction = (targetPoint - castPoint.position);
        direction += Vector3.up * 0.1f; // Small upward angle

        // Instantiate fireball
        GameObject fireball = Instantiate(fireballPrefab, castPoint.position, Quaternion.LookRotation(direction));

    }
    
    public IEnumerator runestoneAbilityMechanic()
    {
        Abilities.setAbilityCancelled(false);//ability canclled starts as false when entering the function
       
        yield return StartCoroutine(placeIndicator(runestonePrefab, runestoneIndicatorPrefab, runestoneMaxCastDistance,2));
        
        if (!Abilities.getAbilitiesCanclled())//if the ability isnt canclled then you use the ability
        {
            if (checkIfManaIsEnough(playerMana, runestoneSkill.getManaCost()))//checks if you have enough mana
            {
                playerMana -= runestoneSkill.getManaCost(); //using ability costs mana
            }
            runestoneSkill.StartCooldown(); 
        }
        else
        {
            runestoneSkill.setCanUseAbility(true); 
        }
    }

    public IEnumerator lightningAbilityMechanic()
    {
        Abilities.setAbilityCancelled(false);//ability canclled starts as false when entering the function

        yield return StartCoroutine(placeIndicator(lightningPrefab, lightningIndicatorPrefab, lightningMaxCastDistance, 2));

        if (!Abilities.getAbilitiesCanclled())//if the ability isnt canclled then you use the ability
        {
            if (checkIfManaIsEnough(playerMana, electricitySkill.getManaCost()))//checks if you have enough mana
            {
                playerMana -= electricitySkill.getManaCost(); //using ability costs mana
            }
            electricitySkill.StartCooldown();
        }
        else
        {
            electricitySkill.setCanUseAbility(true);
        }
    }
  

    public IEnumerator placeIndicator(GameObject abilityPrefab, GameObject abilityIndicatorPrefab, float maxCastDistance,int indicatorHeight)
    {
       
        GameObject abilityIndicator = null;
        Abilities.usingAbility = true; // Doesnt let you cast any abilities
       
       
        while (true)  // Keep casting the ability
        {
            
           
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Ray from center of screen
          
            RaycastHit hit;
            int layerMask = ~(LayerMask.GetMask("Default") | LayerMask.GetMask("InvisibleWall")); // Ignore default layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                usingIndicator = true;

                Vector3 lookAtTower = TowerPos.position - hit.point; // Direction to tower
                lookAtTower.y = 0; // Ignore height

                // Create indicator if it doesn't exist
                if (abilityIndicator == null)
                {
                    //abilityIndicator = Instantiate(abilityIndicatorPrefab, hit.point, Quaternion.LookRotation(lookAtTower)); //indicator can move y axix
                    abilityIndicator = Instantiate(abilityIndicatorPrefab, new Vector3(hit.point.x, indicatorHeight, hit.point.y), Quaternion.LookRotation(lookAtTower));
                    //indicator can't move y axix
                    
                }
              
                abilityIndicator.transform.position = new Vector3(hit.point.x, 0, hit.point.z);//indicator goes to the viewport 

                if (Input.GetKeyDown(KeyCode.Mouse1)&& gameManager.getGameState() != GameStateV2.Paused)//removes indicator when mouse pressed if game is not paused
                {
                    Destroy(abilityIndicator);
                    Abilities.setAbilityCancelled(true);//the ability was canclled
                    Abilities.usingAbility = false;//abilities can once again be used
                    break;
                }
                // Check placement validity
                if (hit.collider.gameObject.layer != 3 || lookAtTower.magnitude > maxCastDistance || lookAtTower.magnitude < 4)
                    SetIndicatorColor(abilityIndicator, Color.red); // Invalid placement
                else
                {
                    SetIndicatorColor(abilityIndicator, Color.blue); // Valid placement

                    // Place ability on key press
                    if (Input.GetKeyDown(KeyCode.Mouse0)&& gameManager.getGameState() != GameStateV2.Paused)
                    {
                        Destroy(abilityIndicator);
                        Instantiate(abilityPrefab, hit.point, Quaternion.LookRotation(lookAtTower));
                        Abilities.usingAbility = false;//abilities can once again be used
                      
                        break;
                    }
                 
                }
            }
           

            yield return null; // Wait for next frame
        }
        usingIndicator = false;
    }

    private void SetIndicatorColor(GameObject indicator, Color color) // Change indicator color
    {
        MeshRenderer renderer = indicator.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.material.color = color;

        // Apply color  to child object
        if (indicator.transform.childCount > 0)
        {
            Transform child = indicator.transform.GetChild(0);
            MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
            if (childRenderer != null) childRenderer.material.color = color;
        }
    }

    IEnumerator ClearAimLine()
    {
        yield return new WaitForSeconds(aimLineDuration);
        aimLine.SetPosition(0, Vector3.zero);
        aimLine.SetPosition(1, Vector3.zero);
    }

    public void abilityCooldownTimer(Abilities ability) //Cooldown For Abilities
    {
        if (ability.getCanUseAbility()) return; //timer starts when player CAN'T use an ability
        float abilityTimer = ability.getTimer();
       
        ability.setTimer(abilityTimer - Time.deltaTime);// Decrease the timer each frame
        if (abilityTimer <= 0f)
        {
            ability.setCanUseAbility(true); // true when player can use ability
        }
    }
    //PLAYER MONEY GETER-SETER
    public int getPlayerMoney()
    {
         return playerMoney;
    }
    public void setPlayerMoney(int playerMoney)
    {
        this.playerMoney = playerMoney;
    }

    //UNLOCK/UPGRADE ABILITIES(FUNC)
    //if player has enough money he can upgrade or purchase an ability
    public void upgradeAbility(Abilities ability) 
    {
        if (notEnoughMoneyToUpgradeAnAbility(ability)) return;
        playerMoney -= ability.getAbilityUpgradeCost();
        ability.setAbilityStatus(AbilityStatus.isUpgraded);
        if (ability.getManaCost() > 0)
        {
            ability.setManaCost(ability.getManaCost() + 15);
        }

    }
    public bool notEnoughMoneyToUpgradeAnAbility(Abilities ability)
    {
        return ability.getAbilityUpgradeCost() > playerMoney;
    }
    public bool notEnoughMoneyToUnlockAnAbility(Abilities ability)
    {
        return ability.getAbilityUnlockCost() > playerMoney;
    }
    public void unlockAbility(Abilities ability)
    {
        if (notEnoughMoneyToUnlockAnAbility(ability)) return;
        playerMoney -= ability.getAbilityUnlockCost();
        ability.setAbilityStatus(AbilityStatus.isUnlocked);

    }

    //GETTERS FOR ABILITIES (UI)

    public Abilities getFireBallAbility()
    {
        return fireBallSkill;
    }
    public Abilities getElectricityAbility()
    {
        return electricitySkill;
    }
    public Abilities getRunestoneAbility()
    {
        return runestoneSkill;
    }
    //Mana
    public void setPlayerMana(int mana)
    {
        if (mana > 100) { mana = 100; }
        if (mana < 0) { mana = 0; }

        playerMana = mana;
    }
    public int getPlayerMana()
    {
       return playerMana; 
    }
    public bool isManaDepleted()
    {
        return getPlayerMana() <= 0;
        
    }
    public bool checkIfManaIsEnough(int myMana, int manaCost)
    {
        return myMana >= manaCost;
        
    }
    public bool checkIfManaIsNotEnoughAfterPressingAnAbility(int myMana, int manaCost)
    {
        return myMana < manaCost && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q));
    
    }
    
}

public class Abilities{  
    public enum AbilityStatus
    {
        isLocked, isUnlocked, isUpgraded

    }
    public static bool usingAbility;//checks if player is using an ability with an indicator
    private static bool abilityCancelled = false;//checks if ability is canclled
    // Private fields
    private bool canUse;
    private float cooldownTime;
    private float timer;
    private int manaCost;
    private int abilityUnlockCost;
    private int abilityUpgradeCost;


    private AbilityStatus abilityStatus;
  

    // Constructor to initialize the ability
    public Abilities(bool abilityCooldownPassed, float cooldownTime, int manaCost,AbilityStatus abilityStatus,int abilityUnlockCost,int abilityUpgradeCost)
    {
        canUse = abilityCooldownPassed;
        this.cooldownTime = cooldownTime;
        timer = 0f;
        this.manaCost = manaCost;
        this.abilityStatus = abilityStatus;
        this.abilityUnlockCost = abilityUnlockCost;
        this.abilityUpgradeCost = abilityUpgradeCost;
    
    }
    public void StartCooldown()
    {
        canUse = false;
        timer = cooldownTime;
    }
    public static void setAbilityCancelled(bool areAbilitiesCancled) {
        abilityCancelled = areAbilitiesCancled;
    }
    public static bool getAbilitiesCanclled()
    {
        return abilityCancelled;
    }
    //Getter for abilityCost
    public int getAbilityUpgradeCost()
    {
        return abilityUpgradeCost;
    } 
    public int getAbilityUnlockCost()
    {
        return abilityUnlockCost;
    }
    // Setter and Getter for canUse
    public void setCanUseAbility(bool canUse)
    {
        this.canUse = canUse;
    }

    public bool getCanUseAbility()
    {
        return canUse;
    }

    // Setter and Getter for cooldownTime
    public void setCooldownTime(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
    }

    public float getCooldownTime()
    {
        return cooldownTime;
    }

    // Setter and Getter for timer
    public void setTimer(float timer)
    {
        this.timer = timer;
    }

    public float getTimer()
    {
        return timer;
    }

    // Setter and Getter for manaCost
    public void setManaCost(int manaCost)
    {
        this.manaCost = manaCost;
    }

    public int getManaCost()
    {
        return manaCost;
    }

    // Setter and Getter for isUnlocked
   public AbilityStatus getAbilityStatus()
    {
        return abilityStatus;
    }
    public void setAbilityStatus(AbilityStatus abilityStatus)
    {
        this.abilityStatus = abilityStatus;
    }
}
