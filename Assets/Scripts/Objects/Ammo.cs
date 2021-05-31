using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Pickupable
{
    //
    public int amount = 1;

    //
    public Type type = Type.Medium;

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

    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        Magnetize();
    }

    /// <summary>
    /// 
    /// </summary>
    override
    public void ExecuteTrigger(PlayerController player)
    {
        player.AddAmmo(type, amount);
    }
}
