using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    ////// VARIABLES //////

    //
    [SerializeField]
    private float health = 300;

    //
    [SerializeField]
    private Tier tier = Tier.Standard;

    //
    [SerializeField]
    private GameObject enemyDrop;

    //
    public enum Tier 
    {
        Standard,
        Elite,
        MiniBoss,
        Boss,
    }


    ////// PRIVATE //////

    /// <summary>
    ///  
    /// </summary>
    private void SpawnLoot()
    {
        // TODO: Choose random loot rarity and preference base loot rarity from player
        LootManager.DropLoot(0, tier, transform);

        // TODO: Spawn enemy specific drop based on rng and rarity
        if (enemyDrop != null) 
        {
            Instantiate(enemyDrop, transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Die()
    {
        SpawnLoot();
        Destroy(gameObject);
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
        if (health <= 0)
        {
            Die();
        }
    }
}
