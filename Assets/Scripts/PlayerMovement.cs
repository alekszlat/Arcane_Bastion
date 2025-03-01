using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5.0f;
    private float horizontalInput;
    private float vertiacalInput;
    private Vector2 turn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        vertiacalInput = Input.GetAxis("Vertical");
        turn.x += Input.GetAxis("Mouse X");

        //Move the player
        transform.Translate(Vector3.forward * Time.deltaTime * speed * vertiacalInput);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
        transform.localRotation = Quaternion.Euler(0, turn.x, 0);
    }
}
