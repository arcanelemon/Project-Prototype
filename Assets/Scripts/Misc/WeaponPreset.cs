using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPreset : Weapon
{
    ////// OVERRIDES //////

    /// <summary>
    /// Outlines necessary compenents that MUST be initialized per weapon preset.
    /// </summary>
    void Start()
    {
        muzzles = GetComponentsInChildren<Transform>()[1].GetComponentsInChildren<Transform>();

        projectile.damage = damage;
        projectile.range = range;
        projectile.ricochette = ricochette;
    }

    /// <summary>
    /// Example of toggling custom aim event. MUST invoke <see cref="HandleADS()"/> when applicable. 
    /// </summary>ß
    override
    public void ToggleAim()
    {
        HandleADS();
        AlternateFire();
    }

    /// <summary>
    /// Example of custom alternative fire mode.
    /// </summary>
    override
    public void AlternateFire()
    {
        varient = varient == Varient.Auto ? Varient.Burst : Varient.Auto;
    }
}
