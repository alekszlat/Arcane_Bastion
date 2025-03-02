using UnityEngine;

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

    private Rigidbody rb;

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

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowFireBall();
        }
    }

    bool IsGrounded()
    {
        // Cast a ray downward to check for ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
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

}
