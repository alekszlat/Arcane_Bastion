using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkeletonArrowBehavior : MonoBehaviour, IDamageable//implements IDamagable witch is an interface used for enemies to deal damage to tower,
{
    [SerializeField] float arrowDamage = 1.5f;
    [SerializeField] int speed;
    private Transform towerPos;
    private Rigidbody rb;
    private Vector3 towerDir;
    
    private void Awake()
    {
        towerPos = GameObject.FindGameObjectWithTag("Target").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
       ShootArrow();
       addArrowGravity();
      
    }
    public void addArrowGravity() //rotates the arrow's z cordinate to make it look like gravity exists
    {
        transform.Rotate(0,0, 8 * Time.deltaTime);
    }
    public void ShootArrow()
    {
        towerDir = (towerPos.position-transform.position).normalized; // the direction from arrow to the tower
        rb.linearVelocity = (towerDir + Vector3.up * 0.2f) * speed;
    }
    public void attack(ref float towerHealth)//implemented by IDamagable
    {
       towerHealth -= arrowDamage;
       Debug.Log(towerHealth);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {    
            Destroy(gameObject);
        }
    }
   
   

}
