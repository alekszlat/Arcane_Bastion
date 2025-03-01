using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] GameObject player;
    private NavMeshAgent agent;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody and NavMeshAgent don't conflict
        rb.isKinematic = true; // Rigidbody should be kinematic initially
    }

    // Update is called once per frame
    void Update()
    {
        if (agent != null && agent.enabled) // ✅ Check if the agent exists AND is enabled
        {
            agent.SetDestination(player.transform.position); // ✅ Safe to set the destination
        }
    }

    public void ApplyKnockback(Vector3 explosionPosition, float force)
    {
        // Stop AI movement
        if (agent != null)
        {
            agent.enabled = false; // Disable pathfinding
        }

        // Enable physics to apply force
        rb.isKinematic = false;
        Vector3 direction = transform.position - explosionPosition; // Direction away from explosion
        direction.y = 0.5f; // Ensure some upward force is applied

        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        // Re-enable AI after delay
        Invoke(nameof(ResetAI), 2f);
    }

    void ResetAI()
    {
        if (agent != null)
        {
            agent.enabled = true; // Re-enable pathfinding
        }
        rb.isKinematic = true; // Disable physics again
    }
}
