using UnityEngine;
using System.Collections;

public class FreezeAuraControl : MonoBehaviour
{
    private GameObject enemy;

    // Update is called once per frame
    void Update()
    {
        transform.position = enemy.transform.position;
    }

    public void EnemyDeath()
    {
        Destroy(gameObject);
    }

    public void setEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }
}
