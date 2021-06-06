using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Pickupable))]
public class Essence : MonoBehaviour
{
    ///// VARIABLES //////
    
    //
    public int amount = 1;

    //

    private Pickupable pickupableComponent;


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        pickupableComponent = GetComponent<Pickupable>();
        pickupableComponent.ExecuteTrigger += Execution;
        pickupableComponent.motion = Pickupable.PickUpMotion.Fly;
    }


    /// <summary>
    /// 
    /// </summary>
    private void Execution()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddEssence(amount);
    }
}
