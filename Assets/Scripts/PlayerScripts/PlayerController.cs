using NUnit.Framework.Constraints;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;
 public class AbilityCooldown
{
    public bool canUse;
    public float cooldownTime;
    public float timer;
     public AbilityCooldown(bool canUse,float cooldownTime) {
        this.canUse = canUse;
        this.cooldownTime = cooldownTime;
        timer = cooldownTime;
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
    public static AbilityCooldown fireBallCooldown = new AbilityCooldown(true, 2);
    public static AbilityCooldown elecrticityCooldown = new AbilityCooldown(false, 3);


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

        if (Input.GetKeyDown(KeyCode.Mouse0) && fireBallCooldown.canUse)
        {
            ThrowFireBall();
            fireBallCooldown.canUse = false;
            fireBallCooldown.timer = fireBallCooldown.cooldownTime;
        }
        abilityCooldownTimer(fireBallCooldown);
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
    public void abilityCooldownTimer(AbilityCooldown ability)
    {
        if (ability.canUse) return;
        ability.timer -= Time.deltaTime;

        if (ability.timer <= 0f)
        {
            ability.canUse = true;

        }
    }
  
}
