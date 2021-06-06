using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    [Header("FOV")]
    [Space(10)]

    //
    [SerializeField]
    private float fov;

    [Space(10)]
    [Header("Zoom")]
    [Space(10)]

    //
    [SerializeField]
    private float zoomSpeed = 10;

    //
    [SerializeField]
    private float standardZoomFOV = 3f;

    //
    [SerializeField]
    private float mediumZoomFOV = 5f;

    //
    [SerializeField]
    private float farZoomFOV = 40f;

    [Space(10)]
    [Header("Head Bobbing")]
    [Space(10)]

    //
    [SerializeField]
    private float walkBobFrequency = 5;

    //
    [SerializeField]
    private float walkBobHorizontalAmplitude = 0.1f;

    //
    [SerializeField]
    private float walkBobVerticalAmplitude = 0.1f;

    //
    [SerializeField]
    private float sprintBobFrequency = 5;

    //
    [SerializeField]
    private float sprintBobHorizontalAmplitude = 0.1f;

    //
    [SerializeField]
    private float sprintBobVerticalAmplitude = 0.1f;

    //
    [SerializeField]
    [Range(0, 1)]
    private float headBobSmoothing = 0.1f;

    [Space(10)]
    [Header("Impact")]
    [Space(10)]

    //
    [SerializeField]
    private float impactSpeed = 1f;

    [SerializeField]
    private float impactOffset = 0.25f;

    [Space(10)]
    [Header("Vignette")]
    [Space(10)]

    //
    [SerializeField]
    private float minVignetteIntensity = 0;

    //
    [SerializeField]
    private float maxVignetteIntensity = 0.4f;

    [Space(10)]
    [Header("Damage")]
    [Space(10)]

    //
    [SerializeField]
    [Range(.1f, 1)]
    private float damageFadeSpeed;

    //
    [SerializeField]
    private bool distortionLinesEnabled;

    //
    [SerializeField]
    private float distortionAmount = 0.01f;

    //
    [SerializeField]
    private float distortionLineAmount = 3;

    //
    [SerializeField]
    private bool digitalDistortionBlocksEnabled;

    //
    [SerializeField]
    private float distortionBlockAmount = 0.1f;

    //
    [SerializeField]
    private bool rgbDistortionEnabled;

    //
    private float targetFOV = 0;

    //
    private float bobFrequency = 5;

    //
    private float bobHorizontalAmplitude = 0.1f;

    //
    private float bobVerticalAmplitude = 0.1f;

    //
    private float bobTime;

    //
    private float distortionAmountTime;

    //
    private float distortionLineAmountTime;

    //
    private float distortionBlockAmountTime;

    //
    private bool shouldBob;

    //
    private bool shouldZoom;

    //
    private bool shouldShake;

    //
    private bool shouldImpact;

    //
    private bool resetImpact;

    //
    private bool resetZoom;

    //
    private bool alterVignette;

    //
    private bool darkenVignette;

    //
    private bool decrementDamageDistortion;

    //
    private bool decrementDangerDistortion;

    //
    private bool dangerModeActive;

    //
    private Vector3 bobCameraPosition;

    //
    private Vector3 zoomTargetPosition = Vector3.zero;

    //
    private Volume volume;

    //
    private Vignette vignette;

    //
    private LimitlessGlitch11 distortionLines;

    //
    private Limitless_Glitch2 digitalDistortionBlocks;

    //
    private Limitless_Glitch3 rgbDistortion;

    //
    private Camera cam;


    ////// OVERIDES /////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        cam = GetComponent<Camera>();
        fov = cam.fieldOfView;
        volume = GameObject.FindObjectOfType<Volume>();
        volume.profile.TryGet<Vignette>(out vignette);
        volume.profile.TryGet<LimitlessGlitch11>(out distortionLines);
        volume.profile.TryGet<Limitless_Glitch2>(out digitalDistortionBlocks);
        volume.profile.TryGet<Limitless_Glitch3>(out rgbDistortion);
        minVignetteIntensity = vignette.intensity.value;

        damageFadeSpeed *= 10;

        distortionAmountTime = distortionAmount * damageFadeSpeed;
        distortionLineAmountTime = distortionLineAmount * damageFadeSpeed;
        distortionBlockAmountTime = distortionBlockAmount * damageFadeSpeed;
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if (shouldBob) 
        {
            PerformBob();
        } 
        
        if (shouldImpact)
        {
            PerformImpact();
        } 
        
        if (!shouldBob && !shouldImpact && transform.localPosition.y != 0) 
        {
            ResetPosition();
        }

        if (alterVignette)
        {
            if (darkenVignette)
            {
                DarkenVingette();
            } else
            {
                ResetVignette();
            }
        }

        if (decrementDamageDistortion) 
        {
            Damage();
        }

        if (shouldZoom)
        {
            if (resetZoom)
            {
                PerformResetZoom();
            }
            else
            {
                PerformZoom();
            }
        }
    }


    ////// PRIVATE //////
    
    /// <summary>
    /// 
    /// </summary>
    private void ResetPosition() 
    {
         transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, impactSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PerformZoom() 
    {
        if (Mathf.Approximately(cam.fieldOfView, fov - targetFOV))
        {
            shouldZoom = false;
        }

        if (resetZoom)
        {
            resetZoom = false;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * .75f * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PerformResetZoom() 
    {
        if (Mathf.Approximately(fov, cam.fieldOfView))
        {
            shouldZoom = false;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, zoomSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// 
    /// 
    /// </summary>
    private void PerformImpact() 
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, -impactOffset, transform.localPosition.z), impactSpeed * Time.time);

        if (transform.localPosition.y == -impactOffset)
        {
            shouldImpact = false;
        }
    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    private void PerformBob()
    {
        bobTime += Time.deltaTime;
        bobCameraPosition = transform.localPosition + CalculateHeadBobOffset(bobTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, bobCameraPosition, headBobSmoothing);

        if ((transform.localPosition - bobCameraPosition).magnitude <= 0.01f)
        {
            transform.localPosition = bobCameraPosition;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private Vector3 CalculateHeadBobOffset(float time) 
    {
        Vector3 offset = Vector3.zero;

        if (time > 0) 
        {
            float horizontalOffset = Mathf.Cos(time * bobFrequency) * bobHorizontalAmplitude;
            float verticalOffset = Mathf.Sin(time * bobFrequency * 2) * bobVerticalAmplitude;

            offset = (transform.right * horizontalOffset) + (transform.up * verticalOffset);
        }

        return offset;
    }


    //////  PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zoom"></param>
    public void Zoom(Weapon.Zoom zoom)
    {
        switch (zoom)
        {
            case Weapon.Zoom.Standard:
                targetFOV = cam.fieldOfView - standardZoomFOV;
                break;
            case Weapon.Zoom.Medium:
                targetFOV = cam.fieldOfView - mediumZoomFOV;
                break;
            case Weapon.Zoom.Far:
                targetFOV = cam.fieldOfView - farZoomFOV;
                break;
        }

        if (resetZoom) 
        {
            resetZoom = false;
        }

        shouldZoom = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetZoom()
    {
        if (!shouldZoom)
        {
            shouldZoom = true;
        }

        resetZoom = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Bob(bool walking)
    {
        if (walking) 
        {
            bobFrequency = walkBobFrequency;
            bobHorizontalAmplitude = walkBobHorizontalAmplitude;
            bobVerticalAmplitude = walkBobVerticalAmplitude;
        } else 
        {
            bobFrequency = sprintBobFrequency;
            bobHorizontalAmplitude = sprintBobHorizontalAmplitude;
            bobVerticalAmplitude = sprintBobVerticalAmplitude;
        }

        shouldBob = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopBob() 
    {
        bobTime = 0;
        shouldBob = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void DarkenVingette()
    {
        if (!alterVignette)
        {
            alterVignette = true;
        }

        if (!darkenVignette)
        {
            darkenVignette = true;
        }

        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, maxVignetteIntensity, 5 * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetVignette()
    {
        if (!alterVignette)
        {
            alterVignette = true;
        }

        if (darkenVignette)
        {
            darkenVignette = false;
        }

        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, minVignetteIntensity, 5 * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Damage() 
    {
        if (!decrementDamageDistortion) 
        {
            if (distortionLinesEnabled)
            {
                distortionLines.enable.SetValue(new BoolParameter(true));
                distortionLines.amount.SetValue(new ClampedFloatParameter(distortionBlockAmount, 0, distortionBlockAmount)); 
                distortionLines.linesAmount.SetValue( new ClampedFloatParameter(distortionLineAmount, 0, distortionLineAmount));
            }

            if (!dangerModeActive)
            {

                if (digitalDistortionBlocksEnabled)
                {
                    digitalDistortionBlocks.enable.SetValue(new BoolParameter(true));
                    digitalDistortionBlocks.amount.SetValue(new ClampedFloatParameter(distortionBlockAmount, 0, distortionBlockAmount));
                }
            }

            decrementDamageDistortion = true;
        } else if (distortionLines.enable.GetValue<bool>() || digitalDistortionBlocks.enable.GetValue<bool>()) 
        {
            if (distortionLinesEnabled) 
            {
                float amount = distortionLines.amount.GetValue<float>();
                distortionLines.amount.SetValue(new ClampedFloatParameter(amount - (distortionAmountTime * Time.deltaTime), 0,
                    distortionAmount)); 
                
                float lineAmount = distortionLines.linesAmount.GetValue<float>();
                distortionLines.linesAmount.SetValue(new ClampedFloatParameter(lineAmount - (distortionLineAmountTime * Time.deltaTime), 0,
                    distortionLineAmount));

                if (amount <= 0 && lineAmount <= 0) 
                {
                    distortionLines.amount.SetValue(new ClampedFloatParameter(0, 0, distortionAmount));
                    distortionLines.linesAmount.SetValue(new ClampedFloatParameter(0, 0, distortionLineAmount));
                    distortionLines.enable.SetValue(new BoolParameter(false));
                }
            }

            if (!dangerModeActive)
            {
                if (digitalDistortionBlocksEnabled)
                {
                    float amount = digitalDistortionBlocks.amount.GetValue<float>();
                    digitalDistortionBlocks.amount = new ClampedFloatParameter(amount - (distortionBlockAmountTime * Time.deltaTime), 0,
                        distortionBlockAmount);

                    if (amount <= 0)
                    {
                        digitalDistortionBlocks.amount = new ClampedFloatParameter(0, 0, distortionBlockAmount);
                        digitalDistortionBlocks.enable.SetValue(new BoolParameter(false));
                    }
                }
            }
        } else 
        {
            decrementDamageDistortion = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DangerMode() 
    {
        if (distortionLinesEnabled)
        {
            distortionLines.enable.SetValue(new BoolParameter(true));
            distortionLines.amount.SetValue(new ClampedFloatParameter(distortionBlockAmount, 0, distortionBlockAmount));
            distortionLines.linesAmount.SetValue(new ClampedFloatParameter(distortionLineAmount, 0, distortionLineAmount));
        }

        if (digitalDistortionBlocksEnabled)
        {
            digitalDistortionBlocks.enable.SetValue(new BoolParameter(true));
            digitalDistortionBlocks.amount.SetValue(new ClampedFloatParameter(distortionBlockAmount, 0, distortionBlockAmount));
        }

        if (rgbDistortionEnabled)
        {
            rgbDistortion.enable.SetValue(new BoolParameter(true));
        }

        dangerModeActive = true;
    }

    public void StopDangerMode() 
    {
        if (distortionLinesEnabled)
        {
            distortionLines.enable.SetValue(new BoolParameter(false));
        }

        if (digitalDistortionBlocksEnabled)
        {
            digitalDistortionBlocks.enable.SetValue(new BoolParameter(false));
        }

        if (rgbDistortionEnabled)
        {
            rgbDistortion.enable.SetValue(new BoolParameter(false));
        }

        dangerModeActive = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Impact() 
    {
        shouldImpact = true;
        vignette.intensity.value = maxVignetteIntensity * 0.65f;
        ResetVignette();
    }


    ////// MUTATORS //////

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetBobActive() 
    {
        return shouldBob;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetVignetteActive()
    {
        return darkenVignette;
    }
}
