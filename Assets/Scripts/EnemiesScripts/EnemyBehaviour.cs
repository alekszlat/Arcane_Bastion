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
    [SerializeField] GameObject ligthningAura;
    [SerializeField] GameObject freezeAura;
    private GameObject ligthningAuraInstance;
    private GameObject freezeAuraInstance;
    protected bool isHitChecker = false; //if enemy is hit by explosion, this is true for 3 seconds
    protected Transform target;
    protected NavMeshAgent agent;
    private Rigidbody rb;
    private float timer;
    private float health;
    private float vunrabilityPercentage = 1f; //percentage of health that is vunrable to explosion
    private Vector3 targetDirection;
    private float originalEnemySpeed;
    protected bool isEnemyHit = false; //while true health bar is visable,and the skeleton can't shoot arrows
    public virtual void  Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
        originalEnemySpeed = agent.speed;
        health = maxHealth;
      

        if (rb != null)
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

    public void resetEffectedEnemyFromRunestone()
    {
        if (agent != null && agent.enabled == true) { 
            agent.speed = originalEnemySpeed;
        }
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
        
        targetDirection = (target.position - gameObject.transform.position).normalized;

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
        isHitChecker = true; //enemy is hit by explosion
        StartCoroutine(isHit(isHitCooldown));//activates bool isHit=true for isHitCooldown(3) seconds

        if (agent != null)
        {
            Debug.Log("disabled");
            agent.enabled = false; // Disable pathfinding
        }
       
        health -= eDamage * vunrabilityPercentage;

        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics
            rb.useGravity = true;
            rb.AddExplosionForce(eForce, ePosition.position, eRadius, eUpwardModifier, ForceMode.Impulse);
        }

        if (health <= 0)
        {
            if(ligthningAuraInstance != null)
            {
                ligthningAuraInstance.GetComponent<LigthningAuraControl>().EnemyDeath();
            }
            destroyFreezeAura();  
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
                isHitChecker = false;
                agent.enabled = true; // Re-enable NavMeshAgent
              
                resetEffectedEnemyFromRunestone();//resets enemy speed to the original enemy speed
                
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
        if (other.gameObject.CompareTag("Target")) {
        if(!agent.enabled) return;
        agent.isStopped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            if (!agent.enabled) return;
            agent.isStopped = false;
        }
    }

    //if enemy is hit  isEnemyHit = true for duration for hitDurationhitDuration
    public virtual IEnumerator isHit(float hitDuration)
    {
        isEnemyHit = true;

        yield return new WaitForSeconds(hitDuration);

        isEnemyHit = false;
        canvas.SetActive(false);
    }

    //Resets the enemy vunrability percentage to 1f after ligthingEffect duration
    public IEnumerator resetVunrabilityPercentage(float ligthingEffect)
    {
        yield return new WaitForSeconds(ligthingEffect);
        Debug.Log("destroyed");
        Destroy(ligthningAuraInstance);
        vunrabilityPercentage = 1f;
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

    public void setVunrabilityPercentage(float newVunrabilityPercentage)
    {
        vunrabilityPercentage = newVunrabilityPercentage;
        ligthningAuraInstance = Instantiate(ligthningAura, transform.position, Quaternion.identity, transform);
        ligthningAuraInstance.GetComponent<LigthningAuraControl>().setEnemy(gameObject);
        StartCoroutine(resetVunrabilityPercentage(5f));
    }

    public void instantiateFreezeAura()
    {
        freezeAuraInstance = Instantiate(freezeAura, transform.position, Quaternion.identity, transform);
        freezeAuraInstance.GetComponent<FreezeAuraControl>().setEnemy(gameObject);
    }

    public void destroyFreezeAura()
    {
        if (freezeAuraInstance != null)
        {
            freezeAuraInstance.GetComponent<FreezeAuraControl>().EnemyDeath();
        }
    }
}
