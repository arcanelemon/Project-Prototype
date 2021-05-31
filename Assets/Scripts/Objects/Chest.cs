using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, Interactable
{
    ////// VARIABLES //////

    //
    [SerializeField]
    private Loot.Rarity chestRarity = Loot.Rarity.Common;

    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        ObjectPool.Instance.SpawnFromPool("Interaction Point", transform.position, transform.rotation).GetComponent<InteractionPoint>().Assign(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Interact() 
    {
        // TODO: play chest open anim.
        List<GameObject> loot = LootManager.OpenChest(0, chestRarity, transform);
        foreach(GameObject item in loot) 
        {
            item.GetComponent<Rigidbody>().AddExplosionForce(200, transform.position, 100, 10);
        }
    }
}
