using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmoteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //
    [SerializeField]
    [Range(0, 3)]
    private int emoteID;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        HUDController.SelectEmote(emoteID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        HUDController.SelectEmote(-1);
    }
}
