using Unity.VisualScripting;
using UnityEngine;

public class FireBallBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] float speed = 20f;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] float explosionForce = 100f;
    [SerializeField] float explosionUpwardModifier = 1f;
    [SerializeField] LayerMask groundLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyExplosionPhysic();
    }

    void ApplyExplosionPhysic()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObjects in colliders)
        {
            Rigidbody rbNearObject = nearbyObjects.GetComponent<Rigidbody>();
            if (rbNearObject != null)
            {
                rbNearObject.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpwardModifier, ForceMode.Impulse);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
