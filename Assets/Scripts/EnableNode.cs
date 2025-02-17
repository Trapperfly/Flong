using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnableNode : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool e = false;
    Image image;
    public TMPro.TMP_Dropdown dropdown;
    private void Start()
    {
        image = GetComponent<Image>();
        Creator.instance.createFloor = (CreateFloor)dropdown.value;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!e)
        {
            Creator.instance.ResetVisuals();
            e = true;
            Creator.instance.floorCreatingEnabled = e;
            image.color = Color.blue;
        }
    }

    public void TurnOffVisual()
    {
        e = false;
        Creator.instance.floorCreatingEnabled = e;
        image.color = Color.white;
    }
    public void ChangePrefab()
    {
        Creator.instance.createFloor = (CreateFloor)dropdown.value;
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
