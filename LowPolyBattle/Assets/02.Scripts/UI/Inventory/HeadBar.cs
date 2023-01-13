using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class HeadBar : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]private Transform tr;
    private Vector2 beginPoint;
    private Vector2 move;
    
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPoint = tr.position;
        move = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        tr.position = beginPoint + (eventData.position - move);
    }

}
