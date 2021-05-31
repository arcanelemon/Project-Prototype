using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    //
    private float fov;

    //
    private float targetFOV = 0;

    //
    private Vector3 targetPosition = Vector3.zero;

    //
    private bool shouldZoom;

    //
    private Camera cam;

    //
    private Volume volume;

    //
    private bool resetZoom;

    //
    private bool alterVignette;

    //
    private bool darkenVignette;

    //
    private Vignette vignette;

    //
    private float minVignetteIntensity = 0;

    //
    private float maxVignetteIntensity = 0.4f;


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
        minVignetteIntensity = vignette.intensity.value;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (shouldZoom)
        {
            if(resetZoom)
            {
                ResetZoom();
            } else
            {
                Zoom(Weapon.Zoom.None);
            }
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
    }


    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="zoom"></param>
    public void Zoom(Weapon.Zoom zoom)
    {
        if (!shouldZoom && zoom != Weapon.Zoom.None)
        {
            switch (zoom)
            {
                case (Weapon.Zoom.Standard):
                    targetFOV = cam.fieldOfView - 1;
                    targetPosition.z += 0.1f;
                    break;
                case (Weapon.Zoom.Medium):
                    targetFOV = cam.fieldOfView - 3;
                    targetPosition.z += 0.3f;
                    break;
                case (Weapon.Zoom.Far):
                    targetFOV = cam.fieldOfView - 35;
                    targetPosition.z += 1.5f;
                    break;
            }

            shouldZoom = true;
        } else if (cam.fieldOfView == fov - targetFOV)
        {
            shouldZoom = false;
        }

        if (resetZoom)
        {
            resetZoom = false;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, 8 * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 8 * Time.deltaTime);
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
        else if (fov == cam.fieldOfView)
        {
            shouldZoom = false;
        }

        if (!resetZoom)
        {
            resetZoom = true;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, 10 * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 10 * Time.deltaTime);
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


    ////// MUTATORS //////

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetVignetteActive()
    {
        return darkenVignette;
    }
}
