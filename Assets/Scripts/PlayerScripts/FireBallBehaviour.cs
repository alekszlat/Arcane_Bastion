using UnityEngine;

public class FireBallBehaviour : MonoBehaviour
{

    [SerializeField] float speed = 20f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float explosionForce = 100f;
    [SerializeField] float explosionUpwardModifier = 1f;
    [SerializeField] float fbDamage = 2f;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    private Rigidbody rb;
    private bool hasHitGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make fireball ignore collisions with invisible walls
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("InvisibleWall"), true);

        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        hasHitGround = false;
    }

    void Update()
    {
        checkGroundCollsion();
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

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Target") || !hasHitGround)
        {
            ApplyExplosionPhysic();
            Debug.Log("Hit enemy or tower");
            Destroy(gameObject);
        }
    }

    void checkGroundCollsion()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, groundLayer))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                hasHitGround = true;
                ApplyExplosionPhysic();
                Debug.Log("Ground hit");
                Destroy(gameObject);
            }
        }
    }
}
