using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LigthningMechanic : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int ariaOfEffect;
    [SerializeField] float effectDuration;
    [SerializeField] float ligthningDuration;
    private float vunrabilityPercentage;
    private PlayerController playerController;



    void Start() //PS RUNESTONE works fine but fireball turns off enemy agent so sometimes enemies remain slowed
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        Abilities lightningSkill = playerController.getElectricityAbility(); //gets lighting object from Abilities class in the player controler script
        Collider[] colliders = Physics.OverlapSphere(transform.position, ariaOfEffect, enemyLayer);
        foreach (Collider nearbyObjects in colliders)
        {
            EnemyBehaviour enemy = nearbyObjects.GetComponent<EnemyBehaviour>();
            if (enemy != null)
            {

                if (lightningSkill.getAbilityStatus() == Abilities.AbilityStatus.isUnlocked)//if ability is unlocked electricity stats are:
                {
                    vunrabilityPercentage = 2;
                }
                else if (lightningSkill.getAbilityStatus() == Abilities.AbilityStatus.isUpgraded)//if ability is upgraded electricity stats are:
                {
                    vunrabilityPercentage = 3;
                }

                enemy.setVunrabilityPercentage(vunrabilityPercentage);
          
            }
        }
    }
}
