using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkeletonArrowBehavior : MonoBehaviour, IDamageable
{
 
    public int speed;
    private Rigidbody rb;
    public float arrowDamage = 1.5f;

    private SkeletonEnemy skeleton;

    private void Awake()
    {
        skeleton = GameObject.FindGameObjectWithTag("Enemy").GetComponent<SkeletonEnemy>();
        rb = GetComponent<Rigidbody>();
      
    }
    void Start()
    {
    
        
    }

    // Update is called once per frame
    void Update()
    {
        ShootArrow();
    }
    public void ShootArrow()
    {

      Vector3 dir = (skeleton.getTarget().position-transform.position).normalized;
       // rb.linearVelocity = new Vector3(dir.x, 3f, dir.z);
        rb.linearVelocity = (dir + Vector3.up * 0.2f) * speed; 


    }

    public void attack(ref float towerHealth)
    {
            towerHealth -= arrowDamage;
        Debug.Log(towerHealth);
       
    }


}
