using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField] float mouseSen;
    [SerializeField] Transform player;
    [SerializeField] Transform cameraPos;
 

    private Vector2 rotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShopUiManager.shopIsOpen) return;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");


        rotation.x += mouseX * mouseSen * Time.deltaTime;
        rotation.y += mouseY * mouseSen * Time.deltaTime;

        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        transform.rotation = Quaternion.Euler(-rotation.y, rotation.x, 0);
        player.transform.rotation = Quaternion.Euler(0, rotation.x, 0);
  
    }

    private void LateUpdate()
    {
        transform.position = cameraPos.position;
    }
}
