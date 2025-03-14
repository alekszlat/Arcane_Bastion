using System;
using UnityEngine;
public interface IDamageable //an interface used for enemies to deal damage to tower
{
    void attack(ref float towerHealth);
}
public class TowerBehaviour : MonoBehaviour
{
    private static float towerHealth;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        towerHealth = 10f; //start tower health
        Debug.Log("in tower class "+towerHealth);
    }
    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Arrow"))
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>(); 

            checkIfTowerIsAttacked(damageable);
            Destroy(other.gameObject);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))//if enemy colides with tower 
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>(); // Find the IDamageable component in the enemy
            checkIfTowerIsAttacked(damageable);
 
        }
    }
    public void checkIfTowerIsAttacked(IDamageable damageable)// Method to check if the tower is attacked and process the attack
    {  
        if (towerHealth <= 0)
        {
            Debug.Log("Tower is destroyd!");
        }
        else
        {
            damageable.attack(ref towerHealth); //attacks while tower has health
        }


    }
}
