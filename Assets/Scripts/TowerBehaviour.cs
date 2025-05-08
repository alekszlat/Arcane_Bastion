using System;
using UnityEngine;
public interface IDamageable //an interface used for enemies to deal damage to tower
{
    void attack(ref float towerHealth);
}
public class TowerBehaviour : MonoBehaviour
{
    private static float towerHealth;
    private float maxTowerHealth;
    private MenuManager menuManager;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxTowerHealth = 15f; //start tower health
        towerHealth = maxTowerHealth;
        menuManager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
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
            menuManager.showDeathScreen(); //calls deathScreen From menu Manager
        }
        else
        {
            damageable.attack(ref towerHealth); //attacks while tower has health
        }
    }
    public float getTowerHealth()
    {
        return towerHealth;
    }
    public float getMaxTowerHealth()
    {
        return maxTowerHealth;
    }
}
