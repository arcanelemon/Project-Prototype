using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AmmoBuilder
{
    ////// VARIABLES //////

    //
    private static int ammoAmount;

    //
    public enum Type
    {
        Light,
        Medium,
        Heavy,
        Special,
    }

    //
    public enum Size
    {
        Standard,
        Large,
    }


    ////// PRIVATE //////

    

    /// <summary>
    /// 
    /// </summary>
    private void InitializeLightAmmo(AmmoBuilder.Size ammoSize)
    {
        if (ammoSize == Size.Standard)
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
    private void InitializeeMediumAmmo(AmmoBuilder.Size ammoSize)
    {
        if (ammoSize == Size.Standard)
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
    private void InitializeHeavyAmmo(AmmoBuilder.Size ammoSize)
    {
        if (ammoSize == Size.Standard)
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
    private void InitializeSpecialAmmo(AmmoBuilder.Size ammoSize)
    {
        if (ammoSize == Size.Standard)
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
    public GameObject[] CreateAmmoPack(AmmoBuilder.Type ammoType, AmmoBuilder.Size ammoSize)
    {
        switch (ammoType)
        {
            case Type.Light:
                InitializeLightAmmo(ammoSize);
                break;
            case Type.Medium:
                InitializeeMediumAmmo(ammoSize);
                break;
            case Type.Heavy:
                InitializeHeavyAmmo(ammoSize);
                break;
            case Type.Special:
                InitializeSpecialAmmo(ammoSize);
                break;
        }

        if (ammoSize == Size.Large)
        {
            //ObjectPool

            // pack.quantity = amount
            // return new Gameobject[1];
        } else
        {
            for (int i = 0; i < ammoAmount; i++)
            {
                //ObjectPool.
            }

            // apply explosive force at transform.
            // return new GameObject[ammoamount];
        }
    }
}
