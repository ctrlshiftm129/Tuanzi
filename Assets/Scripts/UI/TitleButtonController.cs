using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButtonController : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public Action<PointerEventData> pointerEnterEvent;
    public Action<PointerEventData> pointerClickEvent;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnterEvent?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var button = GetComponent<Button>();
        if (button.interactable == false) return;
        pointerClickEvent?.Invoke(eventData);
    }
}
