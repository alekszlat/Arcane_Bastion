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

    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        player = GameObject.FindGameObjectWithTag("Player");
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
        if(other.CompareTag("Enemy") || other.CompareTag("TowerBase") || !hasHitGround)
        {
            ApplyExplosionPhysic();
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
                Destroy(gameObject);
            }
        }
    }
}
