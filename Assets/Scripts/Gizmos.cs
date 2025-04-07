using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gizmos : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    Creator creator = Creator.instance;
    public GizmoSpecific specific;
    public float speed;

    public List<Transform> targets = new();

    public void OnDrag(PointerEventData eventData)
    {
        switch (specific)
        {
            case GizmoSpecific.moveX:
                foreach (Transform t in targets)
                {
                    t.position += new Vector3(Input.mousePositionDelta.x * speed, 0);
                }
                break;
            case GizmoSpecific.moveY:
                foreach (Transform t in targets)
                {
                    t.position += new Vector3(0, Input.mousePositionDelta.y * speed * Time.deltaTime);
                }
                break;
            case GizmoSpecific.moveBoth:
                foreach (Transform t in targets)
                {
                    t.position += Input.mousePositionDelta * speed;
                }
                break;
            case GizmoSpecific.scaleX:
                break;
            case GizmoSpecific.scaleY:
                break;
            case GizmoSpecific.scaleBoth:
                break;
            case GizmoSpecific.rotate:
                break;
            default:
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Creator.instance.hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Creator.instance.hovering = false;
    }
}

public enum GizmoSpecific
{
    moveX, moveY, moveBoth,
    scaleX, scaleY, scaleBoth,
    rotate
}
