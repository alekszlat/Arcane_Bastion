using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;  // Assign your ball prefab in the inspector
    [SerializeField] private Transform spawnPoint;   // Where the ball spawns (e.g., in front of the player)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) // Press 'F' to throw
        {
            ThrowBall();
        }
    }

    void ThrowBall()
    {
        if (ballPrefab != null && spawnPoint != null)
        {
            // Instantiate the ball at the spawn point with player's rotation
            GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
