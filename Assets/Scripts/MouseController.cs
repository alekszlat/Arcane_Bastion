using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Vector2 turn;
    [SerializeField] private GameObject player;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - player.transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");

        transform.position = player.transform.position + offset;
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0.0f);
    }
}
