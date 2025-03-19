using Unity.VisualScripting;
using UnityEngine;

public class FireBallBehaviour : MonoBehaviour
{

    [SerializeField] float speed = 20f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float explosionForce = 100f;
    [SerializeField] float explosionUpwardModifier = 1f;
    [SerializeField] float fbDamage = 2f;
    public LayerMask enemyLayer;
    private Rigidbody rb;

    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void ApplyExplosionPhysic()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (Collider nearbyObjects in colliders)
        {
            EnemyBehaviour enemy = nearbyObjects.GetComponent<EnemyBehaviour>();
            if (enemy != null) {
                enemy.ExplosionPhysic(explosionForce, transform, explosionRadius, explosionUpwardModifier, fbDamage);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f); // Set a semi-transparent red color
        Gizmos.DrawSphere(transform.position, explosionRadius); // Draw a solid sphere
    }

    // The destroy doesn't quite destroy the object
    // Try messing with the collision matrix to see if it's a collision issue
    // Try adding  additional collisions components to the object
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Ground"))
        {
            player.GetComponent<PlayerController>().setFireBallCount();
            ApplyExplosionPhysic();
            Destroy(gameObject);
            Debug.Log("Fireball destroyed " + player.GetComponent<PlayerController>().getFireBallCount());
        }
    }
}
