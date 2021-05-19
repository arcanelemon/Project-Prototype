using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Destructable : MeshDestroy
{
    /// <summary>
    /// 
    /// </summary>
    private const float INSTANTIATION_YIELD_TIME = 0.05f;


    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        //StartCoroutine(yieldInstantionYieldTime());
    }

    /// <summary>
    /// Yields for the <see cref="INSTANTIATION_YIELD_TIME"/> before allowing projectile physics to interact.
    /// </summary>
    /// <returns></returns>
    //private IEnumerator yieldInstantionYieldTime() 
    //{
    //    yield return new WaitForSeconds(INSTANTIATION_YIELD_TIME);
    //    gameObject.layer = 0;
    //    yield break;
    //}
}
