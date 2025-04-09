using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SkeletonEnemy : EnemyBehaviour//extends basic enemy behavior
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float shootingInterval = 2;//time before skeleton can shoot
    private bool canShootArrows=true;
    private LayerMask enemyLayer;
    Vector3 targetDirection;
    private void Awake()
    {
       
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");// turns layer into bit layer mask
        targetDirection = (target.position - transform.position).normalized;
     
    }
 
    public void skeletonRotation()
    {
        Quaternion skeletonLookAtTower = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, skeletonLookAtTower, Time.deltaTime * 3);
       
    }
    
    public override void setEnemyDestination()
    {
     
        float distance = Vector3.Distance(transform.position, target.position);
       
        if (distance < agent.stoppingDistance && raycast(Vector3.down,1.1f))//skeleton stops moving if is past the moving distance and starts shooting
        {
            shootArrows();
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
                arrowPrefab,transform.position+targetDirection *1.2f + Vector3.up * 1.8f,
                Quaternion.LookRotation(targetDirection)
            );

            StartCoroutine(shootingCooldown());//after shoting arrow skeleton has to wait "shootingInterval" seconds
            Destroy(arrow, 3f);//arrow gets destroyed after 3 seconds
        }
    }
    IEnumerator shootingCooldown() 
    {
        canShootArrows = false;
        yield return new WaitForSeconds(shootingInterval);
        canShootArrows = true;
    }
    public bool isAlliedEnemyInFront()//checks if allied enemy is infront
    {
        return Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),40, enemyLayer);
    }
}
