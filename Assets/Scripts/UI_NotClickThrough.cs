using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NotClickThrough : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Creator.instance.hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Creator.instance.hovering = false;
    }
}
