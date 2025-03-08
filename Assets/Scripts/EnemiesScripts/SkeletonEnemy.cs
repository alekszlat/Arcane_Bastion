using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkeletonEnemy : EnemyBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public GameObject arrowPrefab;
    private bool canShootArrows=true;
    public float damage=3;
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
        if (canShootArrows)
        {
            
            GameObject arrow= Instantiate(arrowPrefab, new Vector3(transform.position.x,transform.position.y+1.5f,transform.position.z), Quaternion.identity);
            arrow.transform.LookAt(target);
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
