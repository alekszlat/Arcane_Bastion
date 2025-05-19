using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SkeletonEnemy : EnemyBehaviour//extends basic enemy behavior
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float shootingInterval = 2;//time before skeleton can shoot
    private bool canShootArrows = true;
    private LayerMask enemyLayer;
    Vector3 targetDirectionS;
    Animator animator; // Animator component reference
    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");// turns layer into bit layer mask
        targetDirectionS = (target.position - transform.position).normalized;
        animator = GetComponent<Animator>(); // Get the Animator component from the goblin prefab
    }

    public override void Update()
    {
        base.Update(); // Call the base class Update method
        AnimationController(); // Add meaningful logic to avoid UNT0001
    }

    public void skeletonRotation()
    {
        Quaternion skeletonLookAtTower = Quaternion.LookRotation(targetDirectionS);
        transform.rotation = Quaternion.Slerp(transform.rotation, skeletonLookAtTower, Time.deltaTime * 3);
       
    }
    
    public override void setEnemyDestination()
    {
     
        float distance = Vector3.Distance(transform.position, target.position);
       
        if (distance < agent.stoppingDistance && raycast(Vector3.down,1.1f))//skeleton stops moving if is past the moving distance and starts shooting
        {
            isAttackAnimation = true;
        }

        if (agent != null && agent.enabled == true)
        {
          agent.SetDestination(target.position); //sets destination to target
        }
    
    }
  

    public void shootArrows() {
        if (canShootArrows&&!isEnemyHit&&!isAlliedEnemyInFront())//can shoot enemy hasn't been hit soon,the shooting cooldown has reset, no enemies in front
        {
            GameObject arrow = Instantiate(
                arrowPrefab,transform.position+targetDirectionS *1.2f + Vector3.up * 1.8f,
                Quaternion.LookRotation(targetDirectionS)
            );
            Destroy(arrow, 3f);//arrow gets destroyed after 3 seconds
        }
    }

    public bool isAlliedEnemyInFront()//checks if allied enemy is infront
    {
        return Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),40, enemyLayer);
    }

    void AnimationController()
    {
        if (isHitAnimation)
        {
            animator.SetBool("isShooting", false);
            animator.SetBool("isWalking", true);
            // Unable the animator
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true; // Enable the animator
        }

        if (isAttackAnimation)
        {
            animator.SetBool("isShooting", true); // Trigger the attack animation
            animator.SetBool("isWalking", false);
        }
    }
}
