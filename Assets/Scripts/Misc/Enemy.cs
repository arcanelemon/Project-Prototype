using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float health = 500;

    [SerializeField]
    [Range(0, 50)]
    private int minEssenceDrop;

    [SerializeField]
    [Range (0, 50)]
    private int maxEssenceDrop;

    //
    private bool alive = true;


    ////// PRIVATE //////

    /// <summary>
    ///  
    /// </summary>
    private void SpawnLoot()
    {
        // drop essence
        StartCoroutine(IncrementEssenceSpawn(Random.Range(minEssenceDrop, maxEssenceDrop)));

        // TODO: pull all loot from loot manager. 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numEssenceToSpawn"></param>
    /// <returns></returns>
    private IEnumerator IncrementEssenceSpawn(int numEssenceToSpawn)
    {
        for (int i = 0; i < numEssenceToSpawn; i++)
        {
            ObjectPool.Instance.SpawnFromPool("Essence", transform.position, transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Die()
    {
        alive = false;
        SpawnLoot();
        Destroy(gameObject, 1);
    }


    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(float damage)
    {
        // TODO: Adjust damage based on damage type and player damage modifier
        health -= damage;
        if (health <= 0 && alive)
        {
            Die();
        }
    }


}
