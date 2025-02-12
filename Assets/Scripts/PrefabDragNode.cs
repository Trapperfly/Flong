using UnityEngine;
using UnityEngine.EventSystems;

public class PrefabDragNode : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject prefab;

    public GameObject dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = Instantiate(prefab);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragging.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = null;
    }
}
