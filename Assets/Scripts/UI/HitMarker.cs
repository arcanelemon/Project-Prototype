using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    ////// VARIABLES //////

    //
    private Vector3 position;

    //
    private Vector3 viewPortPoint;

    //
    private RectTransform tickMarkerRect;

    ////// OVERRIDES //////

    // Start is called before the first frame update
    void Start()
    {
        tickMarkerRect = GetComponent<RectTransform>();

        StartCoroutine(TickMarkerCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHitMarkerScreenPosition();
    }

    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator TickMarkerCoroutine()
    {
        UpdateHitMarkerScreenPosition();
        yield return new WaitForSeconds(HUDController.TICK_MARKER_STAY_TIME);
        Destroy(gameObject);
        yield break;
    }

    private void UpdateHitMarkerScreenPosition()
    {
        viewPortPoint = Camera.main.WorldToViewportPoint(position);
        tickMarkerRect.anchorMin = viewPortPoint;
        tickMarkerRect.anchorMax = viewPortPoint;
    }

    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    public void SetHitMarkerWorldPosition(Vector3 position, Transform parent)
    {
        this.position = position;
        transform.SetParent(parent);
    }
}
