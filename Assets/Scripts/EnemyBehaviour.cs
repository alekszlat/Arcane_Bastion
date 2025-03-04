using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] LayerMask layerGround;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private float health = 5; 
    private float attackDamage = 2f;
    private float attackInterval = 2f;
    private float timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if(rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        timer = attackInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent != null && agent.enabled == true)
        {
            agent.SetDestination(target.position);
        }
    }

    //Applying explosion on the enemy
    public void ExplosionPhysic(float eForce, Transform ePosition, float eRadius, float eUpwardModifier, float eDamage)
    {
        if (agent != null)
        {
            agent.enabled = false; // Disable pathfinding
        }

        // Enable physics to apply force
        rb.isKinematic = false;
        rb.useGravity = true;

        health -= eDamage;
        rb.AddExplosionForce(eForce, ePosition.position, eRadius, eUpwardModifier, ForceMode.Impulse);

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        Invoke(nameof(ResetAI), 2f);
    }

    // Reseting the navMeshAgent 
    void ResetAI()
    {

        RaycastHit hitGround;
        if (Physics.Raycast(transform.position, Vector3.down ,out hitGround, 1.1f, layerGround))
        {
            if (agent != null)
            {
                agent.enabled = true; // Re-enable NavMeshAgent
            }

            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics
                rb.useGravity = false;
            }
        }

    }

    public void attack(ref float _health)
    {

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = attackInterval;
            _health -= attackDamage;
            Debug.Log(_health);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!agent.enabled) return;
        agent.isStopped = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!agent.enabled) return;
        agent.isStopped = false;
    }

}
