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
    [SerializeField] protected Transform target;
    [SerializeField] LayerMask layerGround;
    protected NavMeshAgent agent;
    private Rigidbody rb;
    public GameObject canvas;
    public Image healthBar;
    private float timer;
    private float health;
    public float maxHealth = 5;
    public float attackDamage = 2f;
    public float attackInterval = 4f;
    bool isEnemyHit=false;
  
  
    

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
        StartCoroutine(isHit(3));

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
        Debug.Log(isEnemyHit);
    }

  

    void ResetAI()
    {

        RaycastHit hitGround;

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
    IEnumerator isHit(float hitDuration)//enemy isHit for hitDuration
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
    public bool raycast(Vector3 raycastWay)//raycast to check if enemy is on ground
    {
       return raycast(raycastWay, 1.1f);
    }
    public bool raycast(Vector3 raycastWay, RaycastHit hitGround)
    {
        return Physics.Raycast(transform.position, raycastWay, out hitGround, 1.5f, layerGround);//raycast ,just incase we ned to save the hitGround ray 
    }
    public Transform getTarget()
    {
        return target;
    }

  
}
