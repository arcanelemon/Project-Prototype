using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]

public class WeaponPreset : MonoBehaviour
{
    ////// VARIABLES

    Weapon weapon;


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        weapon = GetComponent<Weapon>();
        weapon.WeaponUpdateEvent += AlternateFire;
    }


    ////// PRIVATE //////

    /// <summary>
    /// Example of custom alternative fire mode.
    /// </summary>
    private void AlternateFire()
    {
        if (weapon.state != Weapon.State.Reloading)
        {
            if (weapon.state == Weapon.State.Aiming && weapon.varient != Weapon.Varient.Burst)
            {
                weapon.varient = Weapon.Varient.Burst;
            } else if (weapon.state != Weapon.State.Aiming && weapon.varient != Weapon.Varient.Auto)
            {
                weapon.varient = Weapon.Varient.Auto;
            }
        }
    }
}
