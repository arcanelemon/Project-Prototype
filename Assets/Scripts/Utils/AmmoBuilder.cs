using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AmmoBuilder
{
    ////// VARIABLES //////

    //
    private static int ammoAmount;


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary> 
    private static void InitializeLightAmmo(Ammo.Size ammoSize)
    {
        if (ammoSize == Ammo.Size.Standard)
        {
            ammoAmount = 30;
        }
        else
        {
            ammoAmount = 35;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    private static void InitializeeMediumAmmo(Ammo.Size ammoSize)
    {
        if (ammoSize == Ammo.Size.Standard)
        {
            ammoAmount = 20;
        }
        else
        {
            ammoAmount = 25;
        }


    }

    /// <summary>
    /// 
    /// </summary>
    private static void InitializeHeavyAmmo(Ammo.Size ammoSize)
    {
        if (ammoSize == Ammo.Size.Standard)
        {
            ammoAmount = 10;
        }
        else
        {
            ammoAmount = 15;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    private static void InitializeSpecialAmmo(Ammo.Size ammoSize)
    {
        if (ammoSize == Ammo.Size.Standard)
        {
            ammoAmount = 5;
        }
        else
        {
            ammoAmount = 8;
        }

        // TODO: Load Special Ammo Effect.
    }


    ////// PUBLIC //////

    /// <summary>
    /// Creates an ammopack of the specified type and size to be used by the Loot manager. 
    /// </summary>
    public static GameObject[] CreateAmmoPack(Ammo.Type ammoType, Ammo.Size ammoSize, Transform transform)
    {
        switch (ammoType)
        {
            case Ammo.Type.Light:
                InitializeLightAmmo(ammoSize);
                break;
            case Ammo.Type.Medium:
                InitializeeMediumAmmo(ammoSize);
                break;
            case Ammo.Type.Heavy:
                InitializeHeavyAmmo(ammoSize);
                break;
            case Ammo.Type.Special:
                InitializeSpecialAmmo(ammoSize);
                break;
        }

        if (ammoSize == Ammo.Size.Large)
        {
            GameObject ammoPack = ObjectPool.Instance.SpawnFromPool("Ammo Pack", transform.position, Quaternion.identity);
            ammoPack.GetComponent<Ammo>().amount = ammoAmount;
            GameObject[] returnedObject = new GameObject[1];
            returnedObject[0] = ammoPack;
            return returnedObject;
        } else
        {
            GameObject[] returnedObject = new GameObject[ammoAmount];

            for (int i = 0; i < ammoAmount; i++)
            {
                returnedObject[i] = ObjectPool.Instance.SpawnFromPool("Ammo", transform.position, Quaternion.identity);
            }

            return returnedObject;
        }
    }
}
