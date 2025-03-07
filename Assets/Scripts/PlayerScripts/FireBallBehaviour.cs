using Unity.VisualScripting;
using UnityEngine;

public class FireBallBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] float speed = 20f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float explosionForce = 100f;
    [SerializeField] float explosionUpwardModifier = 1f;
    private float fbDamage = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    void ApplyExplosionPhysic()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObjects in colliders)
        {
            EnemyBehaviour enemy = nearbyObjects.GetComponent<EnemyBehaviour>();
            if (enemy != null) {
                enemy.ExplosionPhysic(explosionForce, transform, explosionRadius, explosionUpwardModifier, fbDamage);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Ground"))
        {
            ApplyExplosionPhysic();
            Destroy(gameObject);
        }
    }
}
