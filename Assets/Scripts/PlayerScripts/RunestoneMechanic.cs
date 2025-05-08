using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Abilities;

public class RunestoneMechanic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerController playerController;
    private List<NavMeshAgent> effectedAgentsList = new List<NavMeshAgent>();
    private List<float> originalEnemySpeedList = new List<float>();
    public int runestoneDuration;


    void Start() //PS RUNESTONE works fine but fireball turns off enemy agent so sometimes enemies remain slowed
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(destroyObject());
    }
    
    public IEnumerator destroyObject()//after duration resets runestone effect and destroys runestone
    {
        yield return new WaitForSeconds(runestoneDuration);
        ressetRunestoneEffect();
        Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBehaviour enemyBehaviour = other.gameObject.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour != null)
            {
                enemyBehaviour.instantiateFreezeAura(); // sets enemy to 50% vunrable
            }
            NavMeshAgent agent = other.gameObject.GetComponent<NavMeshAgent>();
            effectedAgentsList.Add(agent); // adds agents to list so we can reset them later
            originalEnemySpeedList.Add(agent.speed); // adds agents speed to list

            if (playerController.getRunestoneAbility().getAbilityStatus() == AbilityStatus.isUpgraded)//checks if ability is unlocked
            {
                agent.speed = 0;
            }
            else if (playerController.getRunestoneAbility().getAbilityStatus() == AbilityStatus.isUnlocked)
            {
                agent.speed = agent.speed * 0.4f;
            }
        }
    }
    
    public void ressetRunestoneEffect()//resets effects from runestone
    {
         for (int i = 0; i < effectedAgentsList.Count; i++)
         {
             if (effectedAgentsList[i] != null)
             {
                effectedAgentsList[i].speed = originalEnemySpeedList[i];
                effectedAgentsList[i].GetComponent<EnemyBehaviour>().destroyFreezeAura(); // resets enemy to 100% vunrable
            }
         }
    }
}
