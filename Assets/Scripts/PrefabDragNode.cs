using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Creator;

public class PrefabDragNode : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject prefab;
    public List<GameObject> prefabs = new();

    public GameObject dragging;

    public TMPro.TMP_Dropdown dropdown;

    public bool bush;
    public bool lighting;

    private void Start()
    {
        if (!bush) prefab = prefabs[dropdown.value];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = Instantiate(prefab);
        if (bush) dragging.GetComponent<FireflyRespawnPoint>().delayed = true;
        if (bush)
        {
            dragging.GetComponent<FireflyRespawnPoint>().type = (FireflyType)dropdown.value;
        }
        if (lighting) dragging.GetComponent<LightStart>().delayedStart = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragging.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (bush) { 
            dragging.GetComponent<FireflyRespawnPoint>().CustomStart();
            dragging.GetComponent<FireflyRespawnPoint>().delayed = false;
        }
        if (lighting) dragging.GetComponent<LightStart>().delayedStart = false;
        Creator.instance.savedData.Add(new SavedInfo(dragging.transform.gameObject, dragging.transform.position, dragging.transform.localScale, dragging.transform.rotation));
        dragging = null;
    }

    public void ChangePrefab()
    {
        if (!bush) prefab = prefabs[dropdown.value];
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
