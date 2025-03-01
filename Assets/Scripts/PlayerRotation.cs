using Unity.VisualScripting;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private Vector2 turn;
    public float jumpForce = 5f;         // Force of the jump
    private Rigidbody rb;                // Reference to Rigidbody
    private bool isGrounded = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {

        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");

        transform.localRotation = Quaternion.Euler(-turn.y, 0, 0);

        // Check for jump input (space key) and if the character is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Apply jump force
            isGrounded = false;  // Character is in the air
        }    

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  // Character is on the ground
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
