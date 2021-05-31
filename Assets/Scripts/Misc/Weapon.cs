using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEditor;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour, Interactable
{
    ////// STANDARD VARIABLES //////

    //
    public enum Type
    {
        Gun,
        Thrower,
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

    //
    public enum Weight
    {
        Light,
        Medium,
        Heavy,
    }

    //
    public Weight weight = Weight.Medium;

    //
    public Ammo.Type ammoType = Ammo.Type.Medium;

    [Space(10)]
    [Header("General")]
    [Space(10)]

    //
    [Range (15, 100)]
    [SerializeField]
    private float speed = 30000;

    //
    [SerializeField]
    private float damage = 10;

    //
    [Range(1, 9999)]
    [SerializeField]
    private int ammo = 30;

    //
    [Range(0, 1)]
    [SerializeField]
    private float spread = 0;

    //
    [Range(0, 2)]
    [SerializeField]
    private float maxSpread = 0;

    //
    [Range(0, 0.2f)]
    [SerializeField]
    private float recoil = 0;

    // The time it takes for recoil to "settle".
    [Range(0, 1)]
    [SerializeField]
    private float settleTime = 0.2f;

    //
    [Range(1, 5)]
    [SerializeField]
    private float recoilDecrementSpeed = 1;

    //
    [Range(25, 500)]
    [SerializeField]
    private float range = 100;

    //
    [Range(1, 30)]
    [SerializeField]
    private int bulletsPerShot = 1;

    //
    [SerializeField]
    private bool ricochette = false;

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

    [Space(10)]
    [Header("Auto Settings")]
    [Space(10)]

    //
    [Range(60, 1500)]
    [SerializeField]
    private int rateOfFire = 1200;

    [Space(10)]
    [Header("Burst Settings")]
    [Space(10)]

    //
    [Range(1, 5)]
    [SerializeField]
    private int bulletsPerBurst = 1;

    //
    [Range(0.01f, 0.75f)]
    [SerializeField]
    private float burstBulletTime = 0.1f;

    //
    [Range(0, 1)]
    [SerializeField]
    private float burstTime = 0f;

    [Space(10)]
    [Header("Track Settings")]
    [Space(10)]

    //
    [SerializeField]
    private bool track = false;

    //
    [Range(0, 360)]
    [SerializeField]
    private float rotationDegree;

    //
    [Range(0, 360)]
    [SerializeField]
    private float rotationLimit;

    [Space(10)]
    [Header("Components")]
    [Space(10)]

    //
    [SerializeField]
    private GameObject muzzleFlash;

    //
    [SerializeField]
    private GameObject impactEffect;

    //
    [SerializeField]
    private GameObject projectilePrefab;

    // TODO: Remove Custom Attributes. Load from player config.

    [Space(10)]
    [Header("Custom")]
    [Space(10)]

    //
    [SerializeField]
    private GameObject weaponSkin;

    //
    [SerializeField]
    private GameObject weaponCharm;

    [Space(10)]
    [Header("Misc Components")]
    [Space(10)]

    //
    [SerializeField]
    private TextMeshProUGUI ammoCounter;

    //
    [SerializeField]
    private TextMeshProUGUI totalAmmoCounter;

    //
    [SerializeField]
    private Image weaponUnavailableImage;

    //
    [SerializeField]
    private Image weaponInfoBackgroundImage;

    //
    private int currAmmo;

    //
    private float currSpread = 0;

    //
    private float currMaxSpread = 0;

    //
    private float currRecoil = 0;

    //
    private int muzzleTurn = 0;

    //
    private bool decrementRecoil = false;

    //
    private bool opened;

    //
    private bool player;

    //
    private IEnumerator recoilCoroutine;

    //
    private IEnumerator burstCoroutine;

    //
    private Transform[] muzzles;

    //
    private HUDController hudController;

    //
    private InteractionPoint interactionPoint;

    //
    [HideInInspector]
    public enum State
    {
        Idle,
        Reloading,
        Aiming,
    }

    [HideInInspector]
    public State state = State.Idle;

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
        WeaponUpdateEvent += UpdateWeaponCore;
        GameObject interactionPointObject = ObjectPool.Instance.SpawnFromPool("Interaction Point", transform.position, Quaternion.identity);
        interactionPoint = interactionPointObject.GetComponent<InteractionPoint>();
        interactionPoint.Assign(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        currAmmo = ammo;
        currSpread = spread;
        currMaxSpread = maxSpread;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        WeaponUpdateEvent();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Interact()
    {
        GameObject.FindObjectOfType<PlayerController>().AddWeapon(this);
        Debug.Log("Picked up: " + gameObject.name);
    }


    ////// PRIVATE //////


    private void UpdateWeaponCore()
    {
        if (decrementRecoil)
        {
            if (currRecoil <= 0)
            {
                currRecoil = 0;
                decrementRecoil = false;
            }
            else if (currRecoil > 0)
            {
                currRecoil -= Time.deltaTime * recoilDecrementSpeed;
            }
        }
    }

    private IEnumerator WaitForDependencies()
    {
        yield break;
    }

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
        while (currBurstRound > 0 && currAmmo > 0)
        {
            SpawnProjectile();
            currBurstRound--;
            yield return new WaitForSeconds(burstBulletTime);
        }
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ammo"></param>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine(int ammo)
    {
        state = State.Reloading;
        currAmmo = 0;

        // TODO: wait for anim time
        yield return new WaitForSeconds(2);
        currAmmo = ammo;
        state = State.Idle;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SpawnProjectile()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            float xRotationOffset;
            float yRotationOffset;
            Vector3 position = player ? Camera.main.transform.TransformPoint(Vector3.forward * 2) : muzzles[muzzleTurn].position; 
            GameObject newProjectile = ObjectPool.Instance.SpawnFromPool(projectilePrefab.name, position, transform.rotation);
            SetProjectileProperties(newProjectile);
            xRotationOffset = Random.Range(-45, 45) * GetSpread() / 5;
            yRotationOffset = Random.Range(-45, 45) * GetSpread() / 5;
            newProjectile.transform.rotation = Quaternion.Euler(new Vector3(newProjectile.transform.rotation.eulerAngles.x + xRotationOffset, newProjectile.transform.rotation.eulerAngles.y + yRotationOffset, newProjectile.transform.rotation.eulerAngles.z));
            ObjectPool.Instance.SpawnFromPool(muzzleFlash.name, muzzles[muzzleTurn].position, muzzles[muzzleTurn].rotation, transform.parent);

        }

        if (muzzles.Length > 1)
        {
            muzzleTurn = muzzleTurn == muzzles.Length - 1 ? 0 : muzzleTurn + 1;
        }

        if (currRecoil + currSpread < currMaxSpread)
        {
            currRecoil += recoil;
        }

        currAmmo -= 1;
        StartRecoilCoroutine();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectile"></param>
    private void SetProjectileProperties(GameObject projectile)
    {

        Projectile projectileComponent = projectile.GetComponent<Projectile>();

        projectileComponent.speed = speed * 1000;
        projectileComponent.damage = damage;
        projectileComponent.range = range;
        projectileComponent.ricochette = ricochette;
        projectileComponent.track = track;
        projectileComponent.rotationDegree = rotationDegree;
        projectileComponent.rotationLimit = rotationLimit;
        projectileComponent.impactEffect = impactEffect;

        if (player)
        {
            projectileComponent.playerHudController = hudController;
            projectileComponent.source = Projectile.Source.Player;
        }
        else
        {
            projectileComponent.source = Projectile.Source.Enemy;
        }
    }

    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    { 

        // TODO: if settle animation exists, change settle time to settle animation time
        // settleTime = Anim.GetTime();

        
        opened = false;

        player = transform.parent != null && transform.parent.tag is "Player";
        if (player)
        {
            hudController =  GameObject.FindObjectOfType<HUDController>();
            hudController.AssignWeaponInformation(ammoCounter, totalAmmoCounter, weaponUnavailableImage, weaponInfoBackgroundImage);

            // TODO: Load Custom Charm and skin
            if (weaponCharm != null)
            {
                HingeJoint charmAnchor = gameObject.GetComponentInChildren<HingeJoint>();
                Instantiate(weaponCharm, charmAnchor.transform.position, Quaternion.identity, transform);
            }

        }

        muzzles = GetComponentsInChildren<Transform>()[1].GetComponentsInChildren<Transform>();

        Destroy(GetComponent<Rigidbody>());
        GetComponentInChildren<Collider>().enabled = false;
        interactionPoint.gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Drop()
    {
        transform.parent = null;

        // TODO: apply force

        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().AddForce(transform.forward * 3);
        GetComponentInChildren<Collider>().enabled = true;
        interactionPoint.gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Shoot()
    {
        if (state != State.Reloading && currAmmo > 0)
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
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reload(int ammo)
    {
        if (state != State.Reloading)
        {
            StartCoroutine(ReloadCoroutine(ammo));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ToggleADS()
    {
        if (state != State.Aiming)
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

            state = State.Aiming;
        } else
        {
            currMaxSpread = maxSpread;
            state = State.Idle;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PlayAnimIfFirstTime()
    {
        if (!opened)
        {
            // TODO: Play anim. change state to busy duirng anim

            opened = true;
        }
    }

    /// <summary>s
    /// 
    /// </summary>
    /// <returns></returns>
    public float FireRateToSeconds()
    {
        return varient != Varient.Burst ? 1.0f / (rateOfFire / 60) : burstTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return currAmmo == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetUsedAmmo()
    {
        return ammo - currAmmo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetRemainingAmmo()
    {
        return currAmmo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetMagazineSize()
    {
        return ammo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetAmmoPercentage()
    {
        return ((float)currAmmo) / ammo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetROF()
    {
        return rateOfFire;
    }

    /// <summary>
    /// 
    /// </summary>
    public float GetSpread()
    {
        return currSpread + currRecoil >= currMaxSpread ? currMaxSpread : (currSpread + currRecoil);
    }

    /// <summary>
    /// 
    /// </summary>
    public event Action WeaponUpdateEvent;
}
