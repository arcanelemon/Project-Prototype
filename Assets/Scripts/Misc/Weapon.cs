using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    ////// STANDARD VARIABLES //////

    //
    private float currRecoil = 0;

    //
    private float currMaxSpread = 0;

    //
    private int muzzleTurn = 0;

    //
    private bool decrementRecoil = false;

    //
    private IEnumerator recoilCoroutine;

    //
    private IEnumerator burstCoroutine;

    //
    public enum Type
    {
        Gun,
    }


    [Header("Classification")]
    [Space(10)]

    //
    public Type type = Type.Gun;

    //
    public enum Varient
    {
        Auto,
        Burst,
        Semi,
    }

    //
    public Varient varient = Varient.Auto;

    [Space(10)]
    [Header("General")]
    [Space(10)]

    //
    public float damage = 10;

    //
    [Range(1, 9999)]
    public int ammo = 30;

    //
    [Range(0, 1)]
    public float spread = 0;

    //
    [Range(0, 2)]
    public float maxSpread = 0;

    //
    [Range(0, 0.5f)]
    public float recoil = 0;

    // The time it takes for recoil to "settle".
    [Range(0, 1)]
    public float settleTime = 0.2f;

    //
    [Range(25, 500)]
    public float range = 100;

    //
    [Range(1, 30)]
    public int bulletsPerShot = 1;

    //
    public bool ricochette = false;

    //
    public enum Zoom
    {
        Standard,
        Medium,
        Far,
        None,
    }

    //
    public Zoom zoom = Zoom.Standard;

    //
    public Projectile projectile;

    [Space(10)]
    [Header("Auto Settings")]
    [Space(10)]

    //
    [Range(60, 1500)]
    public float rateOfFire = 1200;

    [Space(10)]
    [Header("Burst Settings")]
    [Space(10)]

    //
    [Range(1, 5)]
    public int bulletsPerBurst = 1;

    //
    [Range(0.01f, 0.75f)]
    public float burstBulletTime = 0.1f;

    //
    [Range(0, 1)]
    public float burstTime = 0f;

    //
    [HideInInspector]
    public int currAmmo;

    //
    [HideInInspector]
    public Transform[] muzzles;


    ////// CONSTRUCTOR //////

    /// <summary>
    /// 
    /// </summary>
    public Weapon()
    {

    }


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        currAmmo = ammo;
        currMaxSpread = maxSpread;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (decrementRecoil)
        {
            if (currRecoil <= 0)
            {
                currRecoil = 0;
                decrementRecoil = false;
            } else if (currRecoil > 0)
            {
                currRecoil -= Time.deltaTime * 10;
            }
        }
    }


    ////// PRIVATE //////

    /// <summary>
    /// Decrements recoil after completion
    /// </summary>
    /// <returns></returns>
    private IEnumerator RecoilCoroutine()
    {
        decrementRecoil = false;
        yield return new WaitForSeconds(settleTime);
        decrementRecoil = true;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartRecoilCoroutine()
    {
        StopRecoilCoroutine();
        recoilCoroutine = RecoilCoroutine();
        StartCoroutine(recoilCoroutine);

    }

    /// <summary>
    /// 
    /// </summary>
    private void StopRecoilCoroutine()
    {
        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartBurstCoroutine()
    {
        StopBurstCoroutine();
        burstCoroutine = BurstCoroutine();
        StartCoroutine(burstCoroutine);

    }

    /// <summary>
    /// 
    /// </summary>
    private void StopBurstCoroutine()
    {
        if (burstCoroutine != null)
        {
            StopCoroutine(burstCoroutine);
        }
    }

    /// <summary>
    /// Decrements recoil after completion
    /// </summary>
    /// <returns></returns>
    private IEnumerator BurstCoroutine()
    {
        int currBurstRound = bulletsPerBurst;
        while (currBurstRound > 0)
        {
            SpawnProjectile();
            currBurstRound--;
            yield return new WaitForSeconds(burstBulletTime);
        }
        yield break;
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
    public void Shoot()
    {
        if (currAmmo <= 0)
        {
            
        } else
        {
            switch (varient)
            {
                case (Varient.Burst):
                    StartBurstCoroutine();
                    break;
                default:
                    SpawnProjectile();
                    break;
            }
        }

        // Animator
    }

    /// <summary>
    /// 
    /// </summary>
    public void SpawnProjectile()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            float xRotationOffset;
            float yRotationOffset;

            GameObject newProjectile = Instantiate(projectile.gameObject, muzzles[muzzleTurn].position, muzzles[muzzleTurn].rotation);
            xRotationOffset = spread >= currMaxSpread ? Random.Range(-45, 45) * currMaxSpread / 5 : Random.Range(-45, 45) * (spread + currRecoil) / 5;
            yRotationOffset = spread >= currMaxSpread ? Random.Range(-45, 45) * currMaxSpread / 5 : Random.Range(-45, 45) * (spread + currRecoil) / 5;
            newProjectile.transform.rotation = Quaternion.Euler(new Vector3(newProjectile.transform.rotation.eulerAngles.x + xRotationOffset, newProjectile.transform.rotation.eulerAngles.y + yRotationOffset, newProjectile.transform.rotation.eulerAngles.z));
        }

        if (muzzles.Length > 1)
        {
            muzzleTurn = muzzleTurn == muzzles.Length - 1 ? 0 : muzzleTurn + 1;
        }

        if (currRecoil + spread < currMaxSpread)
        {
            currRecoil += recoil;
        }

        currAmmo -= 1;
        StartRecoilCoroutine();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reload()
    {
        currAmmo = ammo;

        // Animator
    }

    /// <summary>
    /// 
    /// </summary>
    public void HandleADS()
    {
        if (currMaxSpread == maxSpread)
        {
            switch (zoom)
            {
                case (Zoom.Standard):
                    currMaxSpread *= .33f;
                    break;
                case (Zoom.Medium):
                    currMaxSpread *= 0.25f;
                    break;
                case (Zoom.Far):
                    currMaxSpread *= 0.2f;
                    break;
            }
        } else
        {
            currMaxSpread = maxSpread;
        }
    }

    /// <summary>s
    /// 
    /// </summary>
    /// <returns></returns>
    public float FireRateToSeconds()
    {
        return varient != Varient.Burst ? 1 / (rateOfFire / 60) : burstTime;
    }

    /// <summary>s
    /// 
    /// </summary>
    /// <returns></returns>
    public float ZoomToFOV()
    {
        switch (zoom)
        {
            case (Zoom.Standard):
                return 5;
            case (Zoom.Medium):
                return 10;
            case (Zoom.Far):
                return 35;
        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract void ToggleAim();

    /// <summary>
    /// 
    /// </summary>
    public abstract void AlternateFire();
}
