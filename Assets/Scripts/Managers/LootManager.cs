using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    ////// VARIABLES //////

    //
    private static int lootWheelWeight = 0;
    
    //
    private static int commonTableWeight = 0;
    
    //
    private static int rareTableWeight = 0;
    
    //
    private static int epicTableWeight = 0;
    
    //
    private static int legendaryTableWeight = 0;
    
    //
    private static int uniqueTableWeight = 0;

    //
    private static Dictionary<int, Loot.Rarity> lootWheel = new Dictionary<int, Loot.Rarity>
    {
        { 50, Loot.Rarity.Common },
        { 25, Loot.Rarity.Rare },
        { 15, Loot.Rarity.Epic },
        { 9, Loot.Rarity.Legendary },
        { 1, Loot.Rarity.Unique }
    };

    //
    private static Dictionary<int, string> commonTable = new Dictionary<int, string>{
        { 40, "None" },
        { 30, "Ammo" },
        { 20, "Ammo Pack"},
        { 10, "Weapon" }
    };

    //
    private static Dictionary<int, string> rareTable = new Dictionary<int, string>{
        { 40, "Ammo Pack" },
        { 30, "Weapon" },
        { 20, "Ammo"},
        { 10, "None" }
    };

    //
    private static Dictionary<int, string> epicTable = new Dictionary<int, string>{
        { 40, "Item 1" },
        { 30, "Item 2" },
        { 20, "Item 3"},
        { 10, "Item 4" }
    };

    //
    private static Dictionary<int, string> legendaryTable = new Dictionary<int, string>{
        { 40, "Item 1" },
        { 30, "Item 2" },
        { 20, "Item 3"},
        { 10, "Item 4" }
    };

    //
    private static Dictionary<int, string> uniqueTable = new Dictionary<int, string>{
        { 40, "Item 1" },
        { 30, "Item 2" },
        { 20, "Item 3"},
        { 10, "Item 4" }
    };



    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        TallyWeights();
    }


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    private void TallyWeights() 
    {
        foreach (KeyValuePair<int, Loot.Rarity> entry in lootWheel)
        {
            lootWheelWeight += entry.Key;
        }

        foreach (KeyValuePair<int, string> entry in commonTable)
        {
            commonTableWeight += entry.Key;
        }

        foreach (KeyValuePair<int, string> entry in rareTable)
        {
            rareTableWeight += entry.Key;
        }

        foreach (KeyValuePair<int, string> entry in epicTable)
        {
            epicTableWeight += entry.Key;
        }

        foreach (KeyValuePair<int, string> entry in legendaryTable)
        {
            legendaryTableWeight += entry.Key;
        } 
        
        foreach (KeyValuePair<int, string> entry in uniqueTable)
        {
            uniqueTableWeight += entry.Key;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ApplySpreadForce() 
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rarityTier"></param>
    /// <returns></returns>
    private static GameObject PullItemFromDropList(Loot.Rarity rarityTier, Transform spawnPosition) 
    {
        Dictionary<int, string> lootTable;
        int tableWeight;

        switch (rarityTier)
        {
            case Loot.Rarity.Rare:
                lootTable = rareTable;
                tableWeight = rareTableWeight;
                break;
            case Loot.Rarity.Epic:
                lootTable = epicTable;
                tableWeight = epicTableWeight;
                break;
            case Loot.Rarity.Legendary:
                lootTable = legendaryTable;
                tableWeight = legendaryTableWeight;
                break;
            case Loot.Rarity.Unique:
                lootTable = uniqueTable;
                tableWeight = uniqueTableWeight;
                break;
            default:
                lootTable = commonTable;
                tableWeight = commonTableWeight;
                break;
        }

        int spinResult = Random.Range(0, tableWeight);
        foreach (KeyValuePair<int, string> weight in lootTable)
        {
            if (spinResult <= weight.Key)
            {
                return ObjectPool.Instance.SpawnFromPool(weight.Value, spawnPosition.position, spawnPosition.rotation);
            }
            else
            {
                spinResult -= weight.Key;
            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startingRarity"></param>
    private static GameObject SpinWheel(int luck, Transform spawnPosition) 
    {
        int spinResult = Random.Range(luck, lootWheelWeight);
        Loot.Rarity rarityTeir;
        foreach (KeyValuePair<int, Loot.Rarity> weight in lootWheel) 
        {
            if(spinResult <= weight.Key) 
            {
                return PullItemFromDropList(weight.Value, spawnPosition);
            } else 
            {
                spinResult -= weight.Key;
            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    private static Vector3 RandomizePosition(Vector3 pivotPosition) 
    {
       return pivotPosition + Random.insideUnitSphere;
    }

    ////// PUBLIC //////

    /// <summary>
    /// Opens a chest of a specified baseRarity.
    /// </summary>
    /// 
    /// <param name="baseRarity"></param>
    public static List<GameObject> OpenChest(int luck, Loot.Rarity baseRarity, Transform spawnTransform) 
    {
        Debug.Log("Chest Opened");

        // TODO: Take into consideration enemy level when making loot calculations

        Debug.Log("Loot Dropped");
        List<GameObject> lootCollection = new List<GameObject>();

        // TODO: Garuentee drop of rarity instead of increasing amount
        int numEssence = 10;
        int numSpins = 30;
        switch (baseRarity)
        {
            case Loot.Rarity.Rare:
                numEssence = 15;
                numSpins = 50;
                break;
            case Loot.Rarity.Epic:
                numEssence = 30;
                numSpins = 75;
                break;
            case Loot.Rarity.Legendary:
                numEssence = 50;
                numSpins = 100;
                break;
            case Loot.Rarity.Unique:
                numEssence = 100;
                numSpins = 100;
                break;
        }

        for (int i = 0; i < numEssence; i++)
        {
            lootCollection.Add(ObjectPool.Instance.SpawnFromPool("Essence", spawnTransform.position, Quaternion.identity));
        }

        for (int i = 0; i < numSpins; i++)
        {
            GameObject itemDropped = SpinWheel(luck, spawnTransform);
            if (itemDropped != null)
            {
                itemDropped.transform.position = RandomizePosition(spawnTransform.position);
                lootCollection.Add(itemDropped);
            }
        }

        return lootCollection;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="luck">player luck acts as a rarity minimum</param>
    /// <param name="enemyTier"></param>
    public static List<GameObject> DropLoot(int luck, Enemy.Tier enemyTier, Transform spawnTransform) 
    {
        // TODO: Take into consideration enemy level when making loot calculations

        Debug.Log("Loot Dropped");
        List<GameObject> lootCollection = new List<GameObject>();

        // Caculate Essense and number of spins
        int numEssence = 1;
        int numSpins = 3;
        switch (enemyTier) 
        {
            case Enemy.Tier.Elite:
                numEssence = 15;
                numSpins = 15;
                break;
            case Enemy.Tier.MiniBoss:
                numEssence = 30;
                numSpins = 30;
                break;
            case Enemy.Tier.Boss:
                numEssence = 100;
                numSpins = 100;
                break;
        }

        for (int i = 0; i < numEssence; i++) 
        {
            lootCollection.Add(ObjectPool.Instance.SpawnFromPool("Essence", spawnTransform.position, spawnTransform.rotation));
        }

        for (int i = 0; i < numSpins; i++) 
        {
            GameObject itemDropped = SpinWheel(luck, spawnTransform);
            if (itemDropped != null) 
            {
                itemDropped.transform.position = RandomizePosition(spawnTransform.position);

                lootCollection.Add(itemDropped);
            }
        }

        return lootCollection;
    }
}
