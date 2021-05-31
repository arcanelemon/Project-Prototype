using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : Pickupable
{
    //
    public int amount = 1;

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
        player.AddEssence(amount);
    }
}
