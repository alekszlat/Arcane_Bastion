using System;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    private float health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 10f;
        Debug.Log(health);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBehaviour enemyBehaviour = other.GetComponent<EnemyBehaviour>();
            if (health <= 0)
            {
                Debug.Log("Tower is destroyd!");
            }
            else
            {
                enemyBehaviour.attack(ref health);
            }

        }
    }
}
