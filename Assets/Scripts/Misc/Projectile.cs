using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    //
    private Vector3 initialPosition;

    // 
    private Rigidbody rb;

    //
    private Collider col;

    //
    public float damage;

    //
    public float range;

    //
    public float speed;

    //
    public bool ricochette;

    //
    public ParticleSystem impactParticle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;

        rb.AddForce(transform.forward * speed);
        Physics.IgnoreLayerCollision(8, 9);
    }

    // Update is called once per frame
    void Update()
    {
        if (OutOfRange()) 
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        tag = collision.gameObject.tag;

        if ((tag is "Ground" || tag is "Wall") && ricochette)
        {
            Ricochette(collision.contacts[0].normal);
        } else
        {
            if (tag is "Destructable")
            {
                collision.gameObject.GetComponent<Destructable>().DestroyMesh(damage);
            } else if (tag is "Enemy") 
            {
                //collision.gameObject.GetComponent<Enemy>().Break();
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy()
    {
        //Instantiate(impactParticle);
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
    private void Ricochette(Vector3 normal) 
    {
        ricochette = false;
        rb.velocity = Vector3.Reflect(rb.velocity, normal);
        if (rb.velocity.magnitude < speed / 100) 
        {
            rb.AddForce(rb.velocity * speed / 75);
            if (rb.velocity.magnitude < speed / 1000) 
            {
                Destroy(gameObject);
            }
        }
    }
}
