using UnityEngine;
using UnityEngine.Rendering;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float explosionForce = 700f;
    [SerializeField] float explosionUpwardModifier = 1f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {

        ApplyExplosionForce();

        Destroy(gameObject);

    }

    void ApplyExplosionForce()
    {
        // Get all nearby colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply explosion force
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpwardModifier, ForceMode.Impulse);
            }

            // Stop enemy AI and apply knockback
            EnemyControl enemy = nearbyObject.GetComponent<EnemyControl>();
            if (enemy != null)
            {
                enemy.ApplyKnockback(transform.position, explosionForce / 10); // Reduce force to avoid extreme push
            }

        }
    }
}
