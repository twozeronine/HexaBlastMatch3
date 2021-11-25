using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIEventHandler : MonoBehaviour,IDragHandler,IPointerClickHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;

    public void OnPointerClick(PointerEventData eventData) => OnClickHandler?.Invoke(eventData);
    public void OnDrag(PointerEventData eventData) => OnDragHandler?.Invoke(eventData);
}
