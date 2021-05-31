using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    //
    private const float MIN_SIGHT_SIZE = 0.8f;

    //
    private const float MAX_SIGHT_SIZE = 3;

    //
    private const float ROF_BREAK_THRESHOLD = 120;

    [Header("Configure")]
    [Space(10)]

    //
    [SerializeField]
    private Sight sightType = Sight.Default;

    //
    [SerializeField]
    private Tick tickType = Tick.Default;

    //
    [SerializeField]
    private Reload reloadType = Reload.Default;

    [Header("Components")]
    [Space(10)]

    //
    [SerializeField]
    private GameObject defaultSight;

    //
    [SerializeField]
    private GameObject outlineSight;

    //
    [SerializeField]
    private GameObject halfSight;

    //
    [SerializeField]
    private GameObject defaultReload;

    //
    [SerializeField]
    private GameObject revolverReload;

    //
    [SerializeField]
    private GameObject dashedReload;

    //
    [SerializeField]
    private GameObject crossTick;

    //
    [SerializeField]
    private GameObject tickTick;

    //
    [SerializeField]
    private Image weaponSillouhete;

    //
    [SerializeField]
    private TextMeshProUGUI ammoCounter;

    //
    [SerializeField]
    private TextMeshProUGUI totalAmmoCounter;

    //
    [SerializeField]
    private Image sheildImageFill;

    //
    [SerializeField]
    private Image healthImageFill;

    //
    [SerializeField]
    private Image healthImageFollow;

    //
    [SerializeField]
    private GameObject emoteWheel;

    //
    [SerializeField]
    private GameObject dangerIcon;

    //
    [SerializeField]
    private Image[] weaponSlots = new Image[3];

    //
    private GameObject uiSight;

    //
    private GameObject uiReload;

    //
    private GameObject uiHitMarker;

    //
    private enum Sight
    {
        Default,
        Outline,
        Half,
    }

    //
    private enum Tick
    {
        Default,
        Cross,
        Tick,
    }

    //
    private enum Reload
    {
        Default,
        Revolver,
        Dashed,
    }

    //
    private IEnumerator decrementHealthFollowCoroutine;

    //
    private IEnumerator tintSightOnHitCoroutine;

    //
    private PlayerController player;

    //
    private int currentWeaponSlotActive;

    //
    private static int emoteSelected = -1;

    //
    private float decrementMinSpread;

    //
    private bool breakSightCoroutineRunning;

    //
    public const float TICK_MARKER_STAY_TIME = 0.2f;


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        UpdateHUD();
        HUDUpdateTasks += StallForTasks;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        HUDUpdateTasks();
    }


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    private void StartTintSightOnHitCouroutine()
    {
        StopTintSightOnHitCouroutine();
        tintSightOnHitCoroutine = TintSightOnHitCoroutine();
        StartCoroutine(tintSightOnHitCoroutine);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopTintSightOnHitCouroutine()
    {
        if (tintSightOnHitCoroutine != null)
        {
            StopCoroutine(tintSightOnHitCoroutine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator TintSightOnHitCoroutine()
    {
        Image image = uiSight.GetComponentInChildren<Image>();
        float alpha = image.color.a;
        image.color = new Color(Color.red.r, Color.red.b, Color.red.g, alpha);
        yield return new WaitForSeconds(TICK_MARKER_STAY_TIME);
        image.color = new Color(Color.white.r, Color.white.b, Color.white.g, alpha);
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartDecrementHealthFollowCoroutine()
    {
        StopDecrementHealthFollowCoroutine();
        decrementHealthFollowCoroutine = DecrementHealthFollow();
        StartCoroutine(decrementHealthFollowCoroutine);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopDecrementHealthFollowCoroutine()
    {
        if (decrementHealthFollowCoroutine != null)
        {
            StopCoroutine(decrementHealthFollowCoroutine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecrementHealthFollow()
    {
        yield return new WaitForSeconds(.5f);

        while (healthImageFollow.fillAmount != healthImageFill.fillAmount)
        {
            healthImageFollow.fillAmount -= .01f;
            yield return new WaitForSeconds(.01f);

            if (healthImageFollow.fillAmount < healthImageFill.fillAmount + .02f && healthImageFollow.fillAmount > healthImageFill.fillAmount - 0.2f)
            {
                healthImageFollow.fillAmount = healthImageFill.fillAmount;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartBreakSightCoroutine()
    {
        if (!breakSightCoroutineRunning)
        {
            StartCoroutine(BreakSight());
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator BreakSight()
    {
        //breakSightCoroutineRunning = true;
        //Animator uiSightAnimator = uiSight.GetComponent<Animator>();

        //uiSightAnimator.Play("Break", 0);
        //while (uiSightAnimator.GetCurrentAnimatorStateInfo(0).IsName("Break"))
        //{
        //    yield return null;
        //}
        //uiSightAnimator.speed = 1 / (weapon.GetROF() / 60);
        //while (uiSightAnimator.GetCurrentAnimatorStateInfo(0).IsName("Gone"))
        //{
        //    yield return null;
        //}
        //uiSightAnimator.speed = 1;
        //breakSightCoroutineRunning = false;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void ResetReloadScale()
    {
        if (uiReload.transform.localScale != Vector3.one)
        {
            uiReload.transform.localScale = Vector3.Lerp(uiReload.transform.localScale, Vector3.one, 20 * Time.deltaTime);
        } else
        {
            HUDUpdateTasks -= ResetReloadScale;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StallForTasks()
    {
        // noop
    }

    /// <summary>
    /// 
    /// </summary>
    private Action HUDUpdateTasks;


    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    public void UpdateFromPlayerSheild(float sheildFillPercentage)
    {
        sheildImageFill.fillAmount = sheildFillPercentage;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    public void UpdateFromPlayerHealth(float healthFillPercentage)
    {
        if (healthImageFill.fillAmount > healthFillPercentage)
        {
            healthImageFill.fillAmount = healthFillPercentage;
            StartDecrementHealthFollowCoroutine();
        }
        else if (healthImageFill.fillAmount < healthFillPercentage)
        {
            healthImageFill.fillAmount = healthFillPercentage;
            healthImageFollow.fillAmount = healthFillPercentage;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentAmmo"></param>
    public void UpdateCurrentAmmo(int ammo, float ammoPercentage)
    {
        ammoCounter.text = ammo.ToString();

        if (ammo == 0 && ammoCounter.text != "--")
        {
            ammoCounter.text = "--";
            ammoCounter.color = Color.white;
        }
        else if (ammoPercentage <= 0.25f && ammoCounter.color != Color.red)
        {
            ammoCounter.color = Color.red;
        }
        else if (ammoPercentage > 0.25f && ammoCounter.color != Color.white)
        {
            ammoCounter.color = Color.white;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="totalAmmo"></param>
    public void UpdateTotalAmmo(int totalAmmo)
    {
        totalAmmoCounter.text = totalAmmo.ToString();

        if (totalAmmo == 0 && totalAmmoCounter.text != "--")
        {
            totalAmmoCounter.text = "--";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="weaponIndex"></param>
    public void UpdateWeaponEquiped(int weaponIndex)
    {
        weaponSlots[currentWeaponSlotActive].color = Color.gray;
        Image weaponSlot = weaponSlots[weaponIndex];
        weaponSlot.color = Color.black;
        currentWeaponSlotActive = weaponIndex;


        if (!weaponSlot.gameObject.activeInHierarchy)
        {
            weaponSlot.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableDefaultIcon()
    {
        uiReload.SetActive(false);
        uiSight.SetActive(true);
        uiSight.transform.localScale = new Vector3(1 + decrementMinSpread, 1 + decrementMinSpread, 1);
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableReloadIcon(float reloadTime)
    {
        uiReload.transform.localScale = uiSight.transform.localScale;

        ResetReloadScale();

        uiSight.SetActive(false);
        uiReload.SetActive(true);

        Animator uiReloadAnimator = uiReload.GetComponent<Animator>();
        if (uiReloadAnimator.GetCurrentAnimatorStateInfo(0).loop)
        {
            uiReloadAnimator.speed = 1 / reloadTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplayAmmoPickup()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplayEmoteWheel()
    {
        emoteWheel.SetActive(true);
        MouseUtils.ReleaseMouse();
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplayDangerMode()
    {
        dangerIcon.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideEmoteWheel()
    {
        emoteWheel.SetActive(false);
        MouseUtils.LockMouse();
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideDangerMode()
    {
        dangerIcon.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emoteSelected"></param>
    public static void SelectEmote(int _emoteSelected)
    {
        emoteSelected = _emoteSelected;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetEmoteSelected()
    {
        return emoteSelected;
    }

    /// <summary>
    /// 
    /// </summary>
    public void IncrementSightScaleWithRecoil()
    {
        //float scale = Mathf.Clamp(weapon.GetSpread() + 1, MIN_SIGHT_SIZE, MAX_SIGHT_SIZE);
        //uiSight.transform.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateTickMarker(Vector3 position)
    {
        // if tick is sight, tint red, else activate object
        if (uiHitMarker.Equals(uiSight))
        {
            StartTintSightOnHitCouroutine();
        }
        else
        {
            HitMarker hitMarker = ObjectPool.Instance.SpawnFromPool(uiHitMarker.name, transform.position, Quaternion.identity).GetComponent<HitMarker>();
            hitMarker.SetHitMarkerWorldPosition(position, transform);
        }

        // TODO: Play audio from AudioManager
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateHUD()
    {
        // TODO: Update Hud from settings before updating from inspector

        switch (sightType)
        {
            case Sight.Outline:
                uiSight = outlineSight;
                break;
            case Sight.Half:
                uiSight = halfSight;
                break;
            default:
                uiSight = defaultSight;
                break;
        }

        switch (reloadType)
        {
            case Reload.Revolver:
                uiReload = revolverReload;
                break;
            case Reload.Dashed:
                uiReload = dashedReload;
                break;
            default:
                uiReload = defaultReload;
                break;
        }

        switch (tickType)
        {
            case Tick.Cross:
                uiHitMarker = crossTick;
                break;
            case Tick.Tick:
                uiHitMarker = tickTick;
                break;
            default:
                uiHitMarker = uiSight;
                break;
        }
    }
}
