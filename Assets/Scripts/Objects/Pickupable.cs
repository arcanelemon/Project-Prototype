using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody), typeof(Collider))]
public class Pickupable : MonoBehaviour
{
    //
    private const float ROTATION_DEGREE = 360;

    //
    private const float SPEED = 2000;

    //
    private const float MAGNETIZE_DISTANCE_THRESHHOLD = 4;

    //
    private const float MAGNETISM_FORCE = 1;

    //
    private const float MAGNETISM_VELOCITY_CAP = 3;

    //
    private Vector3 previousPlayerPosition;

    //
    private Rigidbody rb;

    //
    private Transform player;

    //
    [HideInInspector]
    public PickUpMotion motion = PickUpMotion.Magnetize;

    //
    public enum PickUpMotion
    {
        Magnetize,
        Fly,
    }

    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (motion == PickUpMotion.Fly) 
        {
            UpdateEvent += TowardsPlayer;
        } else 
        {
            UpdateEvent += Magnetize;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        UpdateEvent();
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
            ExecuteTrigger();
            Destroy(gameObject);
        }
    }


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    public Action UpdateEvent;


    ////// PUBLIC /////

    ///
    /// <summary>
    /// 
    /// </summary>
    public void TowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            Vector3 rotateAmount = Vector3.Cross(transform.forward, direction);
            rb.angularVelocity = rotateAmount * ROTATION_DEGREE;
            rb.velocity = transform.forward * SPEED * Time.deltaTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Magnetize()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MAGNETIZE_DISTANCE_THRESHHOLD)
        {
            if (player.position != previousPlayerPosition || distance < 1.5f) 
            {

                if (rb.velocity.magnitude < MAGNETISM_VELOCITY_CAP)
                {
                    rb.AddForce((player.position - transform.position).normalized * MAGNETISM_FORCE, ForceMode.Impulse);
                }

                previousPlayerPosition = player.position;
                // TODO: play coin jingle audio.
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Action ExecuteTrigger;
}
