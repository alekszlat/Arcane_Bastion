using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LigthningMechanic : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int ariaOfEffect;
    [SerializeField] float vunrabilityPercentage;
    [SerializeField] float effectDuration;
    [SerializeField] float ligthningDuration;


    void Start() //PS RUNESTONE works fine but fireball turns off enemy agent so sometimes enemies remain slowed
    {
        ApplyVunrabilityPercentage();
        StartCoroutine(destroyObject());
    }

    public IEnumerator destroyObject()//after duration resets runestone effect and destroys runestone
    {
        yield return new WaitForSeconds(ligthningDuration);
        ApplyVunrabilityPercentage();
        Destroy(gameObject);
    }

    public void ApplyVunrabilityPercentage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ariaOfEffect, enemyLayer);
        foreach (Collider nearbyObjects in colliders)
        {
            EnemyBehaviour enemy = nearbyObjects.GetComponent<EnemyBehaviour>();
            if (enemy != null)
            {
                enemy.setVunrabilityPercentage(vunrabilityPercentage);
            }
        }
    }
}
