using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SkeletonEnemy : EnemyBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject arrowPrefab;
    private bool canShootArrows=true;
  
  


    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
    }


    public float shootingInterval = 2;
    public override void setEnemyDestination()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance >= 30) { 
            agent.enabled = true; 
        }
        if (distance < 30&&raycast(Vector3.down,1.1f))
        {
            agent.enabled = false;
           
            shootArrows();

        }

        if (agent != null && agent.enabled == true)
        {
          
          agent.SetDestination(target.position); // Задaва новата дестинация
        }
    
    }
   
    

    public void shootArrows() {
        Vector3 targetDirection = (target.position - transform.position).normalized;
        if (canShootArrows&&!isEnemyHit)
        {
            GameObject arrow = Instantiate(
                arrowPrefab,transform.position+targetDirection*1.2f + Vector3.up * 1.8f,
                Quaternion.LookRotation(targetDirection) * Quaternion.Euler(0, 90, 0) // Добавяме 90 градуса по оста Y
            );



            StartCoroutine(shootingCooldown());
            Destroy(arrow, 3f);
        }
    }
    IEnumerator shootingCooldown()
    {
        canShootArrows = false;
        yield return new WaitForSeconds(shootingInterval);
        canShootArrows = true;
    }

    public Vector3 getSkeletonPosition()
    {
        return gameObject.transform.position;
    }
    public bool getCanShootArrows()
    {
        return canShootArrows;
    }
    public void setCanShootArrows(bool canShootArrows)
    {
        this.canShootArrows=canShootArrows;
    }


}
