using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    ////// STANDARD VARIABLES //////

    //
    private int muzzleTurn = 0;

    //
    [Range (0.1f,1)]
    public float rateOfFire = 0.25f;

    //
    public float damage = 10;

    //
    [Range (1, 9999)]
    public float ammo = 20;

    //
    [Range (0, 1)]
    public float spread = 0;

    //
    [Range (25, 500)]
    public float range = 100;

    //
    [Range (1, 30)]
    public int bulletsPerShot = 1;
    
    //
    public bool ricochette = false;

    //
    public enum Type 
    {
        Auto,
        Burst,
        Semi,
    }

    //
    public Type type = Type.Auto;

    //
    public Projectile projectile;

    //
    public Transform[] muzzles;


    ////// CONSTRUCTOR //////

    /// <summary>
    /// 
    /// </summary>
    public Weapon() 
    {
        
    }


    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    public void Drop()
    {
        Instantiate(gameObject);
        GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Impulse);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SpawnProjectile()
    {
        float xRotationOffset;
        float yRotationOffset;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            
            GameObject newProjectile = Instantiate(projectile.gameObject, muzzles[muzzleTurn].position, muzzles[muzzleTurn].rotation);
            xRotationOffset = Random.Range(-45, 45) * spread / 5;
            yRotationOffset = Random.Range(-45, 45) * spread / 5;

            newProjectile.transform.rotation = Quaternion.Euler(new Vector3(newProjectile.transform.rotation.eulerAngles.x + xRotationOffset, newProjectile.transform.rotation.eulerAngles.y + yRotationOffset, newProjectile.transform.rotation.eulerAngles.z));
        }
        if (muzzles.Length > 1)
        {
            muzzleTurn = muzzleTurn == muzzles.Length - 1? 0 : muzzleTurn + 1;
        }

        // TODO: Increment spread by time
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract void AlternateFire();
}
