using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestCubeMelee : MonoBehaviour
{
    private float direction = 1;

    private Vector3 position;

    private Rigidbody rb;

    private void Start()
    {
        position = gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Vector3.Distance(position, transform.position) > 40)
        {
            position = transform.position;
            direction *= -1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(transform.position + (transform.forward * 15 * direction * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag is "Player") 
        {
            collision.gameObject.GetComponent<PlayerController>().DamagePlayer(15);
        }
    }
}
