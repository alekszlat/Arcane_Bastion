using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    [SerializeField] static Abilities fireBallSkill = new Abilities(true, 2, 0, true);//object for fireball ability
    [SerializeField] static Abilities electricitySkill = new Abilities(true, 6, 30, true);
    [SerializeField] static Abilities runestoneSkill = new Abilities(true, 15, 40, false);

    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rb;
    private Animator anim;
    private Camera playerCamera;
    private int playerMana=100;

    private Vector3 raycastOffset = new Vector3(0, 1.1f, 0); // Offset for the raycast origin

    void Start()
    {
        TowerPos = GameObject.FindGameObjectWithTag("Target").transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        playerAbilities();

        //ability cooldown is always in update because it activates only when,it's used
        abilityCooldownTimer(fireBallSkill);
        abilityCooldownTimer(electricitySkill);
        abilityCooldownTimer(runestoneSkill);
        //Debug.Log(playerMana);

    }

    // PLAYER MOVEMENT
    public void playerMovement()//TRY  put animations in a difrent class (not MonoBehaviour)
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Forward and sideways movement with animation management
        transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isWalkingForward", true);
        }
        else
        {
            anim.SetBool("isWalkingForward", false);
        }

        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("isWalkingBack", true);
        }
        else
        {
            anim.SetBool("isWalkingBack", false);
        }

        transform.Translate(Vector3.right * speed * Time.deltaTime * horizontalInput);
        if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("isWalkingLeft", true);
        }
        else
        {
            anim.SetBool("isWalkingLeft", false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isWalkingRight", true);
        }
        else
        {
            anim.SetBool("isWalkingRight", false);
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())    // Check for jump input
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
        if (Abilities.usingAbility == true||isManaDepleted()==true) return; // Prevents casting any abilities if one is already in use

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
            StartCoroutine(lightningAbilityMechanic()); // Start the lightning ability process
            electricitySkill.setCanUseAbility(false); // Prevents reusing ability until cooldown
        }

        // Runestone ability
        if (Input.GetKeyDown(KeyCode.E) && runestoneSkill.getCanUseAbility() && checkIfManaIsEnough(playerMana, electricitySkill.getManaCost()))
        {

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
        if (Physics.Raycast(ray, out hit, fireballMaxCastDistance))
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
            int layerMask = ~LayerMask.GetMask("Default"); // Ignore default layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
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

                if (Input.GetKeyDown(KeyCode.Mouse1))//removes indicator when mouse pressed
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
                    if (Input.GetKeyDown(KeyCode.Mouse0))
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
        if (getPlayerMana() <= 0) { return true; } 
        else
        {
            return false;
        }

    }
    public bool checkIfManaIsEnough(int myMana, int manaCost)
    {
        if (myMana >= manaCost) { return true; }
        else { Debug.Log("NO MANA"); return false; }
    }
}

public class Abilities
{
    public static bool usingAbility;//checks if player is using an ability with an indicator
    private static bool abilityCancelled = false;//checks if ability is canclled
    // Private fields
    private bool canUse;
    private float cooldownTime;
    private float timer;
    private int manaCost;
    private bool isUnlocked;


    // Constructor to initialize the ability
    public Abilities(bool abilityCooldownPassed, float cooldownTime, int manaCost, bool isUnlocked)
    {
        canUse = abilityCooldownPassed;
        this.cooldownTime = cooldownTime;
        timer = 0f;
        this.manaCost = manaCost;
        this.isUnlocked = isUnlocked;
    
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
    public void setIsUnlocked(bool isUnlocked)
    {
        this.isUnlocked = isUnlocked;
    }

    public bool getIsUnlocked()
    {
        return isUnlocked;
    }
}
