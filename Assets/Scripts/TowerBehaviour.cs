using System;
using UnityEngine;
public interface IDamageable
{
    void attack(ref float towerHealth);
}
public class TowerBehaviour : MonoBehaviour
{
    private static float towerHealth;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        towerHealth = 10f;
        Debug.Log("in tower class "+towerHealth);
    }
    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Arrow"))
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>(); 


            checkIfTowerIsAttacked(damageable);
        }
    }
    private void OnTriggerStay(Collider other)
    {
    
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>(); // използвайте GetComponentInParent ако компонентът е на родител
           
               
            checkIfTowerIsAttacked(damageable);
        }
    }
    public void checkIfTowerIsAttacked(IDamageable damageable) {

        if (towerHealth <= 0)
        {
            Debug.Log("Tower is destroyd!");
        }
        else
        {
            damageable.attack(ref towerHealth);
        }


    }
}
