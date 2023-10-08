using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//https://hacchi-man.hatenablog.com/entry/2020/05/06/220000

public class UIDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    // Start is called before the first frame update
    void Awake()
    {

        _rectTransform = transform as RectTransform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;
        Vector2 outPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_rectTransform.parent,
                eventData.position,
                eventData.pressEventCamera,
                out outPos);

        _rectTransform.localPosition = outPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
