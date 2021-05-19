using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{

    //
    private Vector3 initialPosition;

    // 
    private Rigidbody rb;

    //
    public float range;

    //
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (OutOfRange())
        {
            Destroy(gameObject);
        }
        else
        {
            MoveProjectile();
        }
    }


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    private bool OutOfRange()
    {
        return Vector3.Distance(initialPosition, transform.position) > range;
    }

    /// <summary>
    /// 
    /// </summary>
    private void MoveProjectile()
    {
        rb.MovePosition(transform.position + (transform.forward * speed * Time.deltaTime));
    }
}
