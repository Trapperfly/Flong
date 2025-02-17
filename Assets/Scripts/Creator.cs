using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Creator : MonoBehaviour
{
    #region Singleton
    public static Creator instance;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion


    public CreateFloor createFloor;
    public bool floorCreatingEnabled;

    public List<GameObject> dragObjects = new();

    Vector2 worldMousePos;

    public List<EnableNode> enableNodes = new();

    public List<GameObject> bushFireflies = new();

    Transform t;
    Vector2 startPos;

    public Transform debrisParent;

    public bool hovering;

    public class SavedInfo
    {
        public SavedInfo(GameObject prefab, Vector2 pos, Vector3 scale, Quaternion rotation)
        {
            gameObject = prefab;
            position = pos;
            this.scale = scale;
            this.rotation = rotation;
        }
        public GameObject gameObject;
        public Vector2 position;
        public Vector3 scale;
        public Quaternion rotation;
    }

    public List <SavedInfo> data = new();
    public List <SavedInfo> savedData = new();

    int i = 0;
    void Update()
    {
        worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!hovering && floorCreatingEnabled)
        {
            switch (createFloor)
            {
                case CreateFloor.Floor:
                    i = 0;
                    break;
                case CreateFloor.AbsoluteFloor:
                    i = 1;
                    break;
                case CreateFloor.FragileFloor:
                    i = 2;
                    break;
                case CreateFloor.IcyFloor:
                    i = 3;
                    break;
                case CreateFloor.SpikyFloor:
                    i = 4;
                    break;
                default:
                    break;
            }

            if (Input.GetMouseButtonDown(0))
            {
                t = Instantiate(dragObjects[i], worldMousePos, Quaternion.identity, null).transform;
                
                startPos = worldMousePos;
            }
            if (Input.GetMouseButton(0) && t)
            {
                t.position = Vector2.Lerp(startPos, worldMousePos, 0.5f);
                t.localScale = new Vector2
                    (Vector2.Distance
                    (startPos, new Vector2(worldMousePos.x, startPos.y)),
                    Vector2.Distance
                    (startPos, new Vector2(startPos.x, worldMousePos.y)));
            }
            if (Input.GetMouseButtonUp(0) && t)
            {
                savedData.Add(new SavedInfo(t.gameObject, t.position, t.localScale, t.rotation));
                t = null;
            }
        }
        
    }

    public void ResetVisuals()
    {
        foreach (var item in enableNodes)
        {
            item.TurnOffVisual();
        }
    }

    public void StartMap()
    {
        foreach(var item in savedData)
        {
            GameObject saved = Instantiate(item.gameObject);
            data.Add(new SavedInfo(saved, saved.transform.position, saved.transform.localScale, saved.transform.rotation));
            item.gameObject.SetActive(false);
        }
        foreach(var item in bushFireflies)
        {
            item.SetActive(false);
        }
        DragAndFire dnf = DragAndFire.instance;
        dnf.rb.angularVelocity = 0;
        dnf.rb.linearVelocity = Vector2.zero;
    }

    public void Restart()
    {
        foreach(var item in savedData)
        {
            item.gameObject.SetActive(true);
        }
        foreach (var item in bushFireflies)
        {
            item.SetActive(true);
        }
        DragAndFire.instance.Disable();
        ResetVisuals();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i] == null || data[i].gameObject == null) return;
            Destroy(data[i].gameObject);
        }
        data.Clear();
        //for (int i = 0; i < savedData.Count; i++)
        //{
        //    GameObject go = Instantiate(savedData[i].gameObject, savedData[i].position, savedData[i].rotation, null);
        //    data.Add(new SavedInfo(go, go.transform.position, go.transform.localScale, go.transform.rotation));
        //}
        GameObject dp = Instantiate(debrisParent).gameObject;
        Destroy(debrisParent.gameObject);
        debrisParent = dp.transform;

        DragAndFire dnf = DragAndFire.instance;
        dnf.fireflyType = FireflyType.None;
        //if (dnf.transform.GetChild(1).childCount > 0)
        //    Destroy(dnf.transform.GetChild(1).GetChild(0).gameObject);

        dnf.rb.angularVelocity = 0;
        dnf.rb.linearVelocity = Vector2.zero;
    }
}
public enum CreateFloor
{
    Floor,
    AbsoluteFloor,
    FragileFloor,
    IcyFloor,
    SpikyFloor,
}

public enum Tools
{
    Move,
    Scale,
    Rotate,
    Delete,
}
