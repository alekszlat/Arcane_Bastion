using NUnit.Framework.Constraints;
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
     public Abilities(bool canUse,float cooldownTime,int manaCost,bool isUnlocked) {
        this.isUnlocked = isUnlocked;
        this.canUse = canUse;
        this.cooldownTime = cooldownTime;
        timer = cooldownTime;
        this.manaCost = manaCost;

    }
}
public class PlayerController : MonoBehaviour
{

    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f; // Force applied when jumping
    [SerializeField] float raycastDistance = 1.1f; // Distance to check for ground
    [SerializeField] LayerMask groundLayer; // Layer to identify ground objects
    [SerializeField] private GameObject ballPrefab;  // Assign your ball prefab in the inspector
    [SerializeField] private Transform spawnPoint;   // Where the ball spawns 
    private float horizontalInput;
    private float verticalInput;
    public static Abilities fireBallSkill = new Abilities(true, 2,2,true);
    public static Abilities electricitySkill = new Abilities(true,6 ,4, true);
    public static Abilities crystalSkill = new Abilities(true, 15 ,5, true);


    private Rigidbody rb;
    // AbilityCooldown fireball=new AbilityCooldown { canUse = true, cooldownTime = 5f, timer = 5f };без конструктор?


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {


        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
        transform.Translate(Vector3.right * speed * Time.deltaTime * horizontalInput);


        // Check for jump input
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && fireBallSkill.canUse)
        {
            ThrowFireBall();
            fireBallSkill.canUse = false;
            fireBallSkill.timer = fireBallSkill.cooldownTime;
        }
        if(Input.GetKeyDown(KeyCode.Mouse1) && electricitySkill.canUse)
        {
            //missing mechanic
            electricitySkill.canUse = false;
            electricitySkill.timer = electricitySkill.cooldownTime;
        }
        if (Input.GetKeyDown(KeyCode.E) && crystalSkill.canUse)
        {
            //missing mechanic
            crystalSkill.canUse = false;
            crystalSkill.timer = crystalSkill.cooldownTime;
        }
        abilityCooldownTimer(fireBallSkill);
        abilityCooldownTimer(electricitySkill);
        abilityCooldownTimer(crystalSkill);

    }

    bool IsGrounded()
    {
        // Cast a ray downward to check for ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
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

    void ThrowFireBall()
    {

        if (ballPrefab != null && spawnPoint != null)
        {
            // Instantiate the ball at the spawn point with player's rotation
            GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
    public void abilityCooldownTimer(Abilities ability)
    {
        if (ability.canUse) return;
        ability.timer -= Time.deltaTime;

        if (ability.timer <= 0f)
        {
            ability.canUse = true;
           
        }
    }

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
