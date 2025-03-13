using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum enemyStates
{
    normal,isShocked
}
public class EnemyBehaviour : MonoBehaviour,IDamageable
{
   
    [SerializeField] LayerMask layerGround;
    [SerializeField] GameObject canvas;
    [SerializeField] Image healthBar;
    [SerializeField] float maxHealth = 5;
    [SerializeField] float attackDamage = 2f;
    [SerializeField] float attackInterval = 4f;
    [SerializeField] protected int isHitCooldown = 3;
    protected Transform target;
    protected NavMeshAgent agent;
    private Rigidbody rb;
    private float timer;
    private float health;
    protected bool isEnemyHit = false;//while true health bar is visable,and the skeleton can't shoot arrows

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void  Start()
    {
        health = maxHealth;
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
   public virtual void Update()
    {
        setEnemyDestination();
        isEnemyHealthBarVisable();
     
      
    }
    public virtual void setEnemyDestination()
    {
        if (agent != null && agent.enabled == true)
        {
            agent.SetDestination(target.position);
        }
    }
    public void isEnemyHealthBarVisable()//if enemy is hit healthbar is visable
    {
        if (!isEnemyHit) return;
  
            canvas.SetActive(true);
            updateHealthBar(maxHealth, health);
        
    }

    private void updateHealthBar(float maxHealth, float currentHealth) { //updates healthbar
   
        float health = currentHealth / maxHealth;
     
        if (healthBar == null) { Debug.Log("healthBar is null"); }

        healthBar.fillAmount = Mathf.MoveTowards(healthBar.fillAmount, health, 1.5f * Time.deltaTime);//"animation for health"
    

    }
    //Applying explosion on the enemy
    public void ExplosionPhysic(float eForce, Transform ePosition, float eRadius, float eUpwardModifier, float eDamage)
    {
        StartCoroutine(isHit(isHitCooldown));//activates bool isHit=true for isHitCooldown(3) seconds

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

        Invoke(nameof(ResetAI), 3.2f);//3,2 so the enemy can have time to lecaribrate,lower than 2 breaks it

    }

    void ResetAI()
    {

       // RaycastHit hitGround;//if needed

        if (raycast(Vector3.down,3)|| raycast(Vector3.up,3)|| raycast(Vector3.back,3)|| raycast(Vector3.forward,3))//raycasts to check if enemy is on ground
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

    public void attack(ref float towerHealth)
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = attackInterval;
            towerHealth -= attackDamage;
            Debug.Log("enemy beh "+ towerHealth);
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
    public virtual IEnumerator isHit(float hitDuration)//enemy isHit for hitDuration
    {
        isEnemyHit = true;//bool

        yield return new WaitForSeconds(hitDuration);

        isEnemyHit = false;
        canvas.SetActive(false);
    }

    public bool raycast(Vector3 raycastWay,float raycastLenght)//raycast to check if enemy is on ground
    {
        return Physics.Raycast(transform.position, raycastWay, raycastLenght);
    }

    public float GetMaxHealth()
    {
        return health;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        health = newMaxHealth;
    }
}
