using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour,IDamageable
{
   
    [SerializeField] LayerMask layerGround;
    [SerializeField] GameObject canvas;
    [SerializeField] Image healthBar;
    [SerializeField] float maxHealth = 5;
    [SerializeField] float attackDamage = 2f;//enemy attack damage
    [SerializeField] float attackInterval = 4f;
    [SerializeField] protected int isHitCooldown = 3;

    protected Transform target;
    protected NavMeshAgent agent;
    private Rigidbody rb;
    private float timer;
    private float health;
    protected bool isEnemyHit = false;//while true health bar is visable,and the skeleton can't shoot arrows


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
    }
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
        float enemyHeight = gameObject.transform.localScale.y+2.5f;
        canvas.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+enemyHeight, gameObject.transform.position.z);//making the ui stay above the enemy by getting the enemy height
        Vector3 targetDirection = (target.position - gameObject.transform.position).normalized;
        canvas.transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
       //look rotation accepts vector3
        updateHealthBar(maxHealth, health);
        
    }

    //updates healthbar
    private void updateHealthBar(float maxHealth, float currentHealth) {
   
        float health = currentHealth / maxHealth;
     
        if (healthBar == null) { Debug.Log("healthBar is null"); }

        //"animation for health"
        healthBar.fillAmount = Mathf.MoveTowards(healthBar.fillAmount, health, 1.5f * Time.deltaTime);
    
    }
    //Applying explosion on the enemy
    public virtual void ExplosionPhysic(float eForce, Transform ePosition, float eRadius, float eUpwardModifier, float eDamage)
    {
        StartCoroutine(isHit(isHitCooldown));//activates bool isHit=true for isHitCooldown(3) seconds

        if (agent != null)
        {
            agent.enabled = false; // Disable pathfinding
        }
       
        health -= eDamage;

        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics
            rb.useGravity = true;
            rb.AddExplosionForce(eForce, ePosition.position, eRadius, eUpwardModifier, ForceMode.Impulse);
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        //3,2 so the enemy can have time to lecaribrate,lower than 2 breaks it
        Invoke(nameof(ResetAI), 3.2f);
   
    }

    void ResetAI()
    {
        // RaycastHit hitGround;
        //raycasts to check if enemy is on ground
        if (raycast(Vector3.down,3) || raycast(Vector3.up,3) 
            || raycast(Vector3.back,3) || raycast(Vector3.forward,3))
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

    //implemented by IDamagable
    public void attack(ref float towerHealth)
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = attackInterval;
            towerHealth -= attackDamage;
     
        }
    }

    // If enemy comes near the tower, stop the enemy
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

    //if enemy is hit  isEnemyHit = true for duration for hitDurationhitDuration
    public virtual IEnumerator isHit(float hitDuration)
    {
        isEnemyHit = true;

        yield return new WaitForSeconds(hitDuration);

        isEnemyHit = false;
        canvas.SetActive(false);
    }

    //raycast to check if enemy is on ground
    public bool raycast(Vector3 raycastWay,float raycastLenght)
    {
        return Physics.Raycast(transform.position, raycastWay, raycastLenght);
    }

    //GETTERS AND SETTERS
    public float GetMaxHealth()
    {
        return health;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        health = newMaxHealth;
    }

}
