using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnableNode : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool e = false;
    Image image;
    public TMPro.TMP_Dropdown dropdown;
    public bool floor;
    public bool tools;
    private void Start()
    {
        image = GetComponent<Image>();
        if (floor) Creator.instance.ChangeFlooring((CreateFloor)dropdown.value);
        if (tools) Creator.instance.ChangeTool((Tools)dropdown.value);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!e)
        {
            Creator.instance.ResetVisuals();
            e = true;
            if (floor) Creator.instance.floorCreatingEnabled = e;
            if (tools) Creator.instance.toolingEnabled = e;
            image.color = Color.blue;
        }
        else
        {
            TurnOffVisual();
        }
    }

    public void TurnOffVisual()
    {
        e = false;
        if (floor) Creator.instance.floorCreatingEnabled = e;
        if (tools) Creator.instance.toolingEnabled = e;
        image.color = Color.white;
    }
    public void ChangePrefab()
    {
        if (floor) Creator.instance.ChangeFlooring((CreateFloor)dropdown.value);
        if (tools) Creator.instance.ChangeTool((Tools)dropdown.value);
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
