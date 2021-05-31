using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent (typeof(VisualEffect))]
public class CleanUpEffect : MonoBehaviour
{
    ////// VARIABLES //////

    //
    private bool ready;

    //
    private VisualEffect visualEffect;

    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (!ready)
        {
            ready = visualEffect.aliveParticleCount > 0;
        }

        if (visualEffect.aliveParticleCount == 0 && ready)
        {
            visualEffect.Reinit();
            gameObject.SetActive(false);
        }
    }
}
