using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pickupable))]
public class Ammo : MonoBehaviour
{
    private Pickupable pickupableComponent;

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
    private void Awake()
    {
        pickupableComponent = GetComponent<Pickupable>();
        pickupableComponent.ExecuteTrigger += Execution;
        pickupableComponent.motion = Pickupable.PickUpMotion.Magnetize;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Execution()
    {
       GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddAmmo(type, amount);
    }
}
