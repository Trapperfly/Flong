using UnityEngine;
using UnityEngine.EventSystems;

public class EnableNode : MonoBehaviour, IPointerClickHandler
{
    public Create create;
    public void OnPointerClick(PointerEventData eventData)
    {
        Creator.instance.create = create;
    }
}
