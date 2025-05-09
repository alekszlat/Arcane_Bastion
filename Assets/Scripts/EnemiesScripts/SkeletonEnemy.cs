using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SkeletonEnemy : EnemyBehaviour//extends basic enemy behavior
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float shootingInterval = 2;//time before skeleton can shoot
    private LayerMask enemyLayer;
    private Animator anim;
    Vector3 targetDirection;
    override public void Start()
    {
        base.Start();
        //skeleton = GameObject.FindGameObjectWithTag("Skeleton");
        anim = GetComponent<Animator>();
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy"); // turns layer into bit layer mask
        targetDirection = (target.position - transform.position).normalized;
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        deactivateAnimator();
    }

    public void skeletonRotation()
    {
        Quaternion skeletonLookAtTower = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, skeletonLookAtTower, Time.deltaTime * 3);
       
    }
    
    public override void setEnemyDestination()
    {
        float distance = Vector3.Distance(transform.position, target.position);

            if (distance < agent.stoppingDistance && raycast(Vector3.down, 1.1f))//skeleton stops moving if is past the moving distance and starts shooting
            {
                anim.SetBool("isShooting", true);//skeleton animation
            }
            else if (agent != null && agent.enabled == true)
            {
                agent.SetDestination(target.position); //sets destination to target
            }
    
    }

    public void shootArrows()
    {
        if (!isEnemyHit && !isAlliedEnemyInFront())//can shoot enemy hasn't been hit soon,the shooting cooldown has reset, no enemies in front
        {
            GameObject arrow = Instantiate(
                arrowPrefab, transform.position + targetDirection * 1.2f + Vector3.up * 1.8f,
                Quaternion.LookRotation(targetDirection)
            );
            Destroy(arrow, 3f);//arrow gets destroyed after 3 seconds
        }
    }

    public override void ExplosionPhysic(float eForce, Transform ePosition, float eRadius, float eUpwardModifier, float eDamage)
    {
        base.ExplosionPhysic(eForce, ePosition, eRadius, eUpwardModifier, eDamage);
        anim.SetBool("isShooting", false);//skeleton animation
    }

    public bool isAlliedEnemyInFront()//checks if allied enemy is infront
    {
        return Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),40, enemyLayer);
    }

    public void deactivateAnimator()
    {
        if(isHitChecker)
        {
            anim.enabled = false;
        }
        else
        {
            anim.enabled = true;
        }
    }
}
