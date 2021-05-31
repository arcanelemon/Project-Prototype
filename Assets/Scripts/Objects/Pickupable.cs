using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody), typeof(Collider))]
public abstract class Pickupable : MonoBehaviour
{
    //
    private const float ROTATION_DEGREE = 360;

    //
    private const float SPEED = 2000;

    //
    private Rigidbody rb;


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        if (tag is "Player")
        {
            ExecuteTrigger(other.gameObject.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }


    ////// PUBLIC /////
    
    ///
    /// <summary>
    /// 
    /// </summary>
    public void TowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            Vector3 rotateAmount = Vector3.Cross(transform.forward, direction);
            rb.angularVelocity = rotateAmount * ROTATION_DEGREE;
        }

        rb.velocity = transform.forward * SPEED * Time.deltaTime;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Magnetize()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public abstract void ExecuteTrigger(PlayerController player);
}
