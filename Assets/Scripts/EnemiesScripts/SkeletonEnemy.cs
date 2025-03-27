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
    private void Awake()
    {
       
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");// turns layer into bit layer mask
    }

    public override void setEnemyDestination()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance >= 30) { //if distance is >30 enemy moves to target 
            agent.enabled = true; 
        }
        if (distance < 30 && raycast(Vector3.down,1.1f))//skeleton stops moving and starts shooting if distance<30
        {
            agent.enabled = false;   
            shootArrows();
        }

        if (agent != null && agent.enabled == true)
        {
          agent.SetDestination(target.position); //sets destination to target
        }
    
    }
  

    public void shootArrows() {
        Vector3 targetDirection = (target.position - transform.position).normalized;

        if (canShootArrows&&!isEnemyHit&&!isAlliedEnemyInFront())//can shoot enemy hasn't been hit soon,the shooting cooldown has reset, no enemies in front
        {
            GameObject arrow = Instantiate(
                arrowPrefab,transform.position+targetDirection*1.2f + Vector3.up * 1.8f,
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
