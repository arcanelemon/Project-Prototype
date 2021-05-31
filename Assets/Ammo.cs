using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Pickupable
{
    //
    public int amount = 1;

    //
    public AmmoPickup.Type type = AmmoPickup.Type.Medium;

    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        TowardsPlayer();
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
