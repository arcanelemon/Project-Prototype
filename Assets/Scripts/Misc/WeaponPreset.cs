using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPreset : Weapon
{
    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        muzzles = GetComponentsInChildren<Transform>()[1].GetComponentsInChildren<Transform>();

        projectile.damage = damage;
        projectile.range = range;
        projectile.ricochette = ricochette;
    }

    /// <summary>
    /// 
    /// </summary>
    override
    public void AlternateFire()
    {

    }
}
