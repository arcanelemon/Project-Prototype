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

    [SerializeField]
    private GameObject sprintIcon;
    
    [SerializeField]
    private GameObject slideIcon;

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
    private GameObject textChatBackground;

    //
    [SerializeField]
    private InputField textChatInputField;

    //
    [SerializeField]
    private Scrollbar textChatScroll;

    //
    [SerializeField]
    private GameObject dangerIcon;

    //
    [SerializeField]
    private TextMeshProUGUI ammoCounter;
    
    //
    [SerializeField]
    private TextMeshProUGUI weaponName;

    //
    [SerializeField]
    private Transform bulletContainer;

    //
    [SerializeField]
    private GameObject weaponInfoGameObject;

    //
    [SerializeField]
    private Image[] weaponSlots;

    //
    private float minSightScale;

    //
    private float sightScaleAdjustment = 0;

    //
    private PlayerController.MovementState sightState = PlayerController.MovementState.Default;

    //
    private static int emoteSelected = -1;

    //
    private IEnumerator decrementHealthFollowCoroutine;

    //
    private IEnumerator tintSightOnHitCoroutine;

    //
    private Color innuendo = new Color32(165, 177, 194, 150);

    //
    private Color blueGrey = new Color32(119, 140, 163, 250);

    //
    private Image weaponInfoBackgroundImage;

    //
    private Image[] bullets;

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
    private PlayerController player;

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

        // Initialize Variables
        minSightScale = MIN_SIGHT_SIZE;
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
    private void StallForTasks()
    {
        // noop
    }

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
    private void ResetSightScale()
    {
        if (uiSight.transform.localScale.x != minSightScale)
        {
            uiSight.transform.localScale = Vector3.Lerp(uiSight.transform.localScale, new Vector3(minSightScale, minSightScale, 0), 20 * Time.deltaTime);
        }
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

        if (healthFillPercentage > 0.2f && dangerIcon.activeInHierarchy) 
        {
            dangerIcon.SetActive(false);
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

        int j = 0;
        for(int i = bullets.Length - 1; i >= 0; i--) 
        {
            if (i > ammo - 1) 
            {
                bullets[j].color = Color.black;
            } else 
            {
                bullets[j].color = Color.white;
            }

            j++;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="horizontalRecoil"></param>
    /// <param name="verticalRecoil"></param>
    public void SetReticleScaleFromRecoil(float horizontalRecoil, float verticalRecoil) 
    {
        float increase = horizontalRecoil > verticalRecoil ? horizontalRecoil : verticalRecoil;
        float scale = Mathf.Clamp(minSightScale + increase, minSightScale, MAX_SIGHT_SIZE);
        uiSight.transform.localScale = new Vector3(scale, scale, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moving"></param>
    public void SetReticleScaleFromMovementState(bool moving, PlayerController.MovementState movementState = PlayerController.MovementState.Default) 
    {

        if (moving) 
        {
            if (movementState != sightState) 
            {
                switch (sightState)
                {
                    case PlayerController.MovementState.Crouched:
                        minSightScale -= 0.1f;
                        break;
                    case PlayerController.MovementState.Jumping:
                        minSightScale -= 0.4f;
                        break;
                    default:
                        minSightScale -= 0.3f;
                        break;
                }

                switch (movementState)
                {
                    case PlayerController.MovementState.Crouched:
                        sightScaleAdjustment = 0.1f;
                        break;
                    case PlayerController.MovementState.Jumping:
                        sightScaleAdjustment = 0.4f;
                        break;
                    default:
                        sightScaleAdjustment = 0.3f;
                        break;
                }

                minSightScale += sightScaleAdjustment;
                sightState = movementState;
            }
        } else 
        {
            minSightScale -= sightScaleAdjustment;
        }
    }
    
    public void SetReticleScaleFromAimState(bool aiming) 
    {
        minSightScale = aiming ? minSightScale / 1.3f : minSightScale * 1.3f;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableDefaultIcon()
    {
        uiReload.SetActive(false);
        uiSight.SetActive(true);
        //uiSight.transform.localScale = new Vector3(1 , 1, 1);
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableReloadIcon(float reloadTime)
    {
        uiReload.transform.localScale = uiSight.transform.localScale;
        HUDUpdateTasks += ResetReloadScale;

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
    public void EnableTextChat() 
    {
        textChatBackground.SetActive(true);
        textChatScroll.interactable = true;
        textChatInputField.interactable = true;
        textChatInputField.ActivateInputField();
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisableTextChat() 
    {
        textChatBackground.SetActive(false);
        textChatScroll.interactable = false;
        textChatInputField.interactable = false;
        textChatInputField.DeactivateInputField();
    }

    /// <summary>
    /// 
    /// </summary>
    public int SendTextChatMessage()
    {
        if (textChatInputField.text.Length > 0)
        {
            string message = textChatInputField.text;
            textChatInputField.text = "";
            DisableTextChat();
            return 1;
        } else
        {
            return 0;
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
    public void DisplaySprintIcon() 
    {
        sprintIcon.SetActive(true);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void DisplaySlideIcon() 
    {
        slideIcon.SetActive(true);
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
    public void HideSprintIcon() 
    {
        sprintIcon.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideSlideIcon() 
    {
        slideIcon.SetActive(false);
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
    /// <returns></returns>
    public bool GetTextChatActive()
    {
        return textChatBackground.activeInHierarchy;
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
    /// <param name="weaponInfoBackgroundImage"></param>
    /// <param name="weaponName"></param>
    /// <param name="weaponAmmoCapacity"></param>
    public void AssignWeaponInformation(Image weaponInfoBackgroundImage, string weaponName, int weaponAmmoCapacity) 
    {
        this.weaponInfoBackgroundImage = weaponInfoBackgroundImage;
        this.weaponName.text = weaponName;
        weaponInfoGameObject.SetActive(true);

        if (bulletContainer.childCount > 0) 
        {
            foreach (Transform child in bulletContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        bullets = new Image[weaponAmmoCapacity];
        for (int i = 0; i < weaponAmmoCapacity; i++) 
        {
            bullets[i] = Instantiate(Resources.Load<GameObject>("Prefabs/UI/HUD/Components/UI Bullet"), bulletContainer).GetComponent<Image>();
        }

        HUDUpdateTasks += ResetSightScale;
        EnableDefaultIcon();
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
