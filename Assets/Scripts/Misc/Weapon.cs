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
        Launcher,
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
    [SerializeField]
    private bool hitScan;

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
    [SerializeField]
    private float damage = 10;

    //
    [Range(1, 9999)]
    [SerializeField]
    private int ammo = 30;

    //
    [Range(60, 1500)]
    [SerializeField]
    private int rateOfFire = 1200;

    //
    [Range(25, 500)]
    [SerializeField]
    private float range = 100;

    //
    [Range(0, 1)]
    [SerializeField]
    private float spread = 0;

    //
    [Range(1, 30)]
    [SerializeField]
    private int bulletsPerShot = 1;

    [Space(10)]

    //
    [Range(0, 1)]
    public float verticalRecoil = 0;

    //
    [Range(0, 1)]
    public float horizontalRecoil = 0;
    
    //
    [Range(1, 10)]
    public float maxVerticalRecoil = 0;

    //
    [Range(1, 10)]
    public float maxHorizontalRecoil = 0;

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
    [Header("Projectile Settings")]
    [Space(10)]

    //
    [Range(15, 100)]
    [SerializeField]
    private float speed = 30000;

    //
    [SerializeField]
    private GameObject projectilePrefab;

    [Space(10)]

    //
    [SerializeField]
    private bool ricochette = false;

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

    // TODO: Remove Custom Attributes. Load from player config.

    [Space(20)]
    [Header("Custom")]
    [Space(10)]

    //
    [SerializeField]
    private GameObject weaponSkin;

    //
    [SerializeField]
    private GameObject weaponCharm;

    [Space(10)]
    [Header("UI Components")]
    [Space(10)]

    //
    [SerializeField]
    private GameObject weaponInformation;

    //
    [SerializeField]
    private TextMeshProUGUI ammoCounter;

    //
    [SerializeField]
    private GameObject ammoGameObject;

    //
    [SerializeField]
    private TextMeshProUGUI totalAmmoCounter;

    //
    [SerializeField]
    private GameObject weaponUnavailable;

    //
    [SerializeField]
    private Image weaponInfoBackgroundImage;

    //
    private int currAmmo;

    //
    private int muzzleTurn = 0;

    //
    private bool opened;

    //
    private bool player;

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
        
    }

    private IEnumerator WaitForDependencies()
    {
        yield break;
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
        ObjectPool.Instance.SpawnFromPool(muzzleFlash.name, muzzles[muzzleTurn].position, muzzles[muzzleTurn].rotation, transform.parent);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float xRotationOffset = Random.Range(-45, 45) * spread / 5;
            float yRotationOffset = Random.Range(-45, 45) *  spread / 5;
            Vector3 position = player ? Camera.main.transform.TransformPoint(Vector3.forward * 2) : muzzles[muzzleTurn].position;

            if (hitScan) 
            {
                Vector3 direction = Quaternion.Euler(new Vector3(0, xRotationOffset, yRotationOffset)) * transform.forward;

                if (Physics.Raycast(position, direction, out RaycastHit hit, range))
                {
                    if (hit.collider.tag is "Enemy" && player)
                    {
                        if (hudController != null)
                        {
                            hudController.CreateTickMarker(hit.transform.position);
                        }

                        // TODO: check if hitbox critical or critcal shot

                        hit.collider.GetComponent<Enemy>().Damage(damage);
                    }
                    else if (hit.collider.tag == "Player" && !player)
                    {
                        hit.collider.GetComponent<PlayerController>().DamagePlayer(damage);
                    }

                    ObjectPool.Instance.SpawnFromPool(impactEffect.name, hit.point, Quaternion.identity);
                }
            } else 
            {
                GameObject newProjectile = ObjectPool.Instance.SpawnFromPool(projectilePrefab.name, position, transform.rotation);
                newProjectile.transform.rotation = Quaternion.Euler(new Vector3(newProjectile.transform.rotation.eulerAngles.x + xRotationOffset, newProjectile.transform.rotation.eulerAngles.y + yRotationOffset, newProjectile.transform.rotation.eulerAngles.z));
                SetProjectileProperties(newProjectile);
            }
        }

        if (muzzles.Length > 1)
        {
            muzzleTurn = muzzleTurn == muzzles.Length - 1 ? 0 : muzzleTurn + 1;
        }

        currAmmo -= 1;
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
            AssignWeaponUIToHUD();

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
        weaponInformation.SetActive(true);
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
        weaponInformation.SetActive(false);
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
                case Varient.Burst:
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
                case Zoom.Standard:
                    break;
                case Zoom.Medium:
                    break;
                case Zoom.Far:
                    break;
            }

            state = State.Aiming;
        } else
        {
            state = State.Idle;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void AssignWeaponUIToHUD() 
    {
        hudController.AssignWeaponInformation(ammoCounter, totalAmmoCounter, ammoGameObject, weaponUnavailable, weaponInfoBackgroundImage);
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
    public event Action WeaponUpdateEvent;
}
