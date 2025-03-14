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
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f; // Force applied when jumping
    [SerializeField] float raycastDistance = 1.1f; // Distance to check for ground
    [SerializeField] LayerMask groundLayer; // Layer to identify ground objects
    [SerializeField] private GameObject ballPrefab;  // Assign your ball prefab in the inspector
    [SerializeField] private Transform spawnPoint;   // Where the ball spawns 
    [SerializeField] static Abilities fireBallSkill = new Abilities(true, 2, 2, true);//object for fireball ability
    [SerializeField] static Abilities electricitySkill = new Abilities(true, 6, 4, true);
    [SerializeField] static Abilities crystalSkill = new Abilities(true, 15, 5, true);
    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rb;
  
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        playerAbilities();
        abilityCooldownTimer(fireBallSkill);//ability cooldown is always in update because it activates only when,it's used
        abilityCooldownTimer(electricitySkill);
        abilityCooldownTimer(crystalSkill);

        if (Input.GetButtonDown("Jump") && IsGrounded())    // Check for jump input
        {
            Jump();
        }

    }
    public void playerAbilities() {
        if (Input.GetKeyDown(KeyCode.Mouse0) && fireBallSkill.canUse)
        {
            ThrowFireBall();
            fireBallSkill.canUse = false;//if ability is used it cant be used again until cooldown is over
            fireBallSkill.timer = fireBallSkill.cooldownTime;
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
    public void playerMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * speed * Time.deltaTime * verticalInput);
        transform.Translate(Vector3.right * speed * Time.deltaTime * horizontalInput);
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
    public void abilityCooldownTimer(Abilities ability) //cooldown for fireBall skill
    {
        if (ability.canUse) return; //timer starts when player CAN'T use an ability
            ability.timer -= Time.deltaTime;

        if (ability.timer <= 0f)
        {
            ability.canUse = true; // true when player can use ability
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
