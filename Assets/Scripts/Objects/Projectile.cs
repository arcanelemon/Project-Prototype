using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, PooledObject
{
    //
    private float distance;

    //
    private Vector3 initialPosition;

    // 
    private Rigidbody rb;

    //
    private TrailRenderer trailRenderer;

    //
    [HideInInspector]
    public float damage;

    //
    [HideInInspector]
    public float range;

    //
    [HideInInspector]
    public float speed;

    //
    [HideInInspector]
    public bool ricochette;

    //
    [HideInInspector]
    public bool track;

    //
    [HideInInspector]
    public float rotationDegree;

    //
    [HideInInspector]
    public float rotationLimit;

    //
    [HideInInspector]
    public GameObject impactEffect;

    //
    [HideInInspector]
    public HUDController playerHudController;

    //
    [HideInInspector]
    public Source source = Source.Enemy;

    //
    public enum Source
    {
        Player,
        Enemy,
    }

    // Start is called before the first frame update
    public void OnObjectSpawn()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        initialPosition = transform.position;
        rb.velocity = Vector3.zero;

        // stagger immediate trail render to prevent strange behavior when strafing
        trailRenderer.enabled = false;

        if (source == Source.Player)
        {
            Physics.IgnoreLayerCollision(8, 9);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        distance = CalculateDistance(initialPosition);

        if (speed != 0 && rb.velocity.magnitude == 0)
        {
            rb.velocity = transform.forward * speed * Time.deltaTime;
        }

        if (distance > range)
        {
            gameObject.SetActive(false);
        }
        else if (!trailRenderer.enabled && distance > 1f)
        {
            EnableTrail();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        if (track)
        {
            TowardsNearestEnemy();
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
            } else if (tag is "Enemy" && source == Source.Player) 
            {
                if(playerHudController != null)
                {
                    playerHudController.CreateTickMarker(transform.position);
                }

                // TODO: check if hitbox critical or critcal shot

                collision.gameObject.GetComponent<Enemy>().Damage(damage);
            } else if (tag == "Player" && source == Source.Enemy)
            {
                collision.gameObject.GetComponent<PlayerController>().DamagePlayer(damage);
            }

            ObjectPool.Instance.SpawnFromPool(impactEffect.name, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Disable()
    {

    }


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private float CalculateDistance(Vector3 targetPosition) 
    {
        return Vector3.Distance(targetPosition, transform.position);
    }

    /// <summary>
    /// 
    /// </summary>
    private void EnableTrail()
    {
        trailRenderer.enabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Ricochette(Vector3 normal) 
    {
        ricochette = false;
        Vector3 direction = Vector3.Reflect(rb.velocity, normal);
        rb.velocity = direction * (speed / rb.velocity.magnitude) * Time.deltaTime;        
        if (!trailRenderer.enabled)
        {
            EnableTrail();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void TowardsNearestEnemy()
    {
        GameObject[] enemies = source == Source.Player ? GameObject.FindGameObjectsWithTag("Enemy") : GameObject.FindGameObjectsWithTag("Player");

        GameObject targetEnemy = null;
        float closestDistance = range;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = CalculateDistance(enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                targetEnemy = enemy;
            }
        }

        if (targetEnemy != null && Vector3.Angle(transform.position, targetEnemy.transform.position) < rotationLimit)
        {
            Vector3 direction = targetEnemy.transform.position - transform.position;
            direction.Normalize();
            Vector3 rotateAmount = Vector3.Cross(transform.forward, direction);
            rb.angularVelocity = rotateAmount * rotationDegree;
        }

        rb.velocity = transform.forward * speed * Time.deltaTime;
    }
}
