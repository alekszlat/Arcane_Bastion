using NUnit.Framework.Constraints;
using System.Collections;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
 public class Abilities
{
    public bool canUse;
    public float cooldownTime;
    public float timer;
    public int manaCost;
    bool isUnlocked;
     public Abilities(bool canUse,float cooldownTime,int manaCost,bool isUnlocked) {//class for abilities that checks if an ability is usable,timer,manaCost, and if its unlocked
        this.isUnlocked = isUnlocked;
        this.canUse = canUse;
        this.cooldownTime = cooldownTime;
        timer = cooldownTime;
        this.manaCost = manaCost;

    }
}
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f; // Force applied when jumping
    [SerializeField] float raycastDistance = 1.1f; // Distance to check for ground

    [Header("Fireball Ability")]
    [SerializeField] private GameObject fireballPrefab;  // Assign your ball prefab in the inspector
    [SerializeField] private Transform castPoint;   // Where the ball spawns
    [SerializeField] float maxCastDistance = 100f;
    // Add a LineRenderer or UI reticle to show aim
    [SerializeField] LineRenderer aimLine;
    [SerializeField] float aimLineDuration = 0.1f;

    [Header("Player Abilities")]
    [SerializeField] static Abilities fireBallSkill = new Abilities(true, 2, 2, true);//object for fireball ability
    [SerializeField] static Abilities electricitySkill = new Abilities(true, 6, 4, true);
    [SerializeField] static Abilities crystalSkill = new Abilities(true, 15, 5, true);

    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rb;
    private Animator anim;
    private Camera playerCamera;

    private Vector3 raycastOffset = new Vector3(0, 1.1f, 0); // Offset for the raycast origin

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        playerAbilities();
        abilityCooldownTimer(fireBallSkill);//ability cooldown is always in update because it activates only when,it's used
        abilityCooldownTimer(electricitySkill);
        abilityCooldownTimer(crystalSkill);

    }

    // PLAYER MOVEMENT
    public void playerMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Forward and sideways movement with animation management
        transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
        if(Input.GetKey(KeyCode.W))
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
    public void playerAbilities() {
        if (Input.GetKeyDown(KeyCode.Mouse0) && fireBallSkill.canUse)
        {
            fireBallSkill.canUse = false;//if ability is used it cant be used again until cooldown is over
            anim.SetBool("isFireBallCasting", true);
            fireBallSkill.timer = fireBallSkill.cooldownTime;
        }
        else
        {
            anim.SetBool("isFireBallCasting", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && electricitySkill.canUse)
        {
            //TODO:missing eletricity skill

            electricitySkill.canUse = false;
            electricitySkill.timer = electricitySkill.cooldownTime;
        }
        if (Input.GetKeyDown(KeyCode.E) && crystalSkill.canUse)
        {
            //TODO:missing cristal skill
            crystalSkill.canUse = false;
            crystalSkill.timer = crystalSkill.cooldownTime;
        }
    }

    public void abilityCooldownTimer(Abilities ability) //cooldown for fireBall skill
    {
        if (ability.canUse) return; //timer starts when player CAN'T use an ability
        ability.timer -= Time.deltaTime;

        if (ability.timer <= 0f)
        {
            ability.canUse = true; // true when player can use ability
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
        if (Physics.Raycast(ray, out hit, maxCastDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxCastDistance);
        }

        // Debugging with LineRenderer
        if (aimLine != null)
        {
            aimLine.SetPosition(0, castPoint.position);
            aimLine.SetPosition(1, targetPoint);
            StartCoroutine(ClearAimLine());
        }

        // Calculate direction with slight upward arc
        // Try without normalization to see the difference
        Vector3 direction = (targetPoint - castPoint.position);
        direction += Vector3.up * 0.1f; // Small upward angle

        // Instantiate fireball
        GameObject fireball = Instantiate(fireballPrefab, castPoint.position, Quaternion.LookRotation(direction));

    }

    IEnumerator ClearAimLine()
    {
        yield return new WaitForSeconds(aimLineDuration);
        aimLine.SetPosition(0, Vector3.zero);
        aimLine.SetPosition(1, Vector3.zero);
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
    public Abilities getCristalAbility()
    {
        return crystalSkill;
    }
}
