using UnityEngine;
using System.Collections;

public class LigthningAuraControl : MonoBehaviour
{
    private GameObject enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start the coroutine to destroy the object after a certain duration
        StartCoroutine(destroyObject());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = enemy.transform.position;
    }

    public IEnumerator destroyObject()//after duration resets runestone effect and destroys runestone
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void EnemyDeath()
    {
        DestroyImmediate(gameObject);
    }

    public void setEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }
}
