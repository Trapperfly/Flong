using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEditor.Overlays;
using UnityEngine;

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
    public Tools tool;
    public bool floorCreatingEnabled;
    public bool toolingEnabled;
    public bool moving;
    public bool rotating;
    public bool scaling;
    public bool deleting;

    public LayerMask objectAlteringMask;

    public List<GameObject> dragObjects = new();

    Vector2 worldMousePos;

    public List<EnableNode> enableNodes = new();

    public List<GameObject> bushFireflies = new();

    Transform t;
    Vector2 startPos;

    public Transform debrisParent;

    public bool hovering;

    public CinemachineCamera cam;

    public GameObject[] fireflyPrefabs;
    [System.Serializable]
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

    public List<SavedInfo> toBeRemoved = new();

    public TMPro.TMP_InputField input;

    public Transform levelParent;
    public Transform activeLevelParent;

    int floor = 0;
    public void ChangeFlooring(CreateFloor floorType)
    {
        switch (floorType)
        {
            case CreateFloor.Floor:
                floor = 0;
                break;
            case CreateFloor.AbsoluteFloor:
                floor = 1;
                break;
            case CreateFloor.FragileFloor:
                floor = 2;
                break;
            case CreateFloor.IcyFloor:
                floor = 3;
                break;
            case CreateFloor.SpikyFloor:
                floor = 4;
                break;
            default:
                break;
        }
    }
    int tools = 0;
    public void ChangeTool(Tools toolType)
    {
        switch (toolType)
        {
            case Tools.Move:
                tools = 0;
                break;
            case Tools.Scale:
                tools = 1;
                break;
            case Tools.Rotate:
                tools = 2;
                break;
            case Tools.Delete:
                tools = 3;
                break;
            default:
                break;
        }
    }
    void Update()
    {
        worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!hovering && floorCreatingEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                t = Instantiate(dragObjects[floor], worldMousePos, Quaternion.identity, null).transform;
                
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
        
        if (!hovering && toolingEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (tools == 3)//Delete
                {
                    Collider2D[] hits = Physics2D.OverlapCircleAll(worldMousePos, 0.1f);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i] != null)
                        {
                            for (int j = 0; j < savedData.Count; j++)
                            {
                                if (savedData[j].gameObject == hits[i].gameObject) savedData.RemoveAt(j);
                            }
                            hits[i].TryGetComponent(out Firefly fly);
                            if (fly)
                                if (fly.respawnPoint)
                                    Destroy(fly.respawnPoint.gameObject);
                            Destroy(hits[i].gameObject);
                        }
                    }
                }
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
            if (item.gameObject == null || item.scale.x == 0 || item.scale.y == 0) {
                toBeRemoved.Add(item);
                return;
            }
            if (item.gameObject.transform.parent == null) { item.gameObject.transform.parent = levelParent; }
            GameObject saved = Instantiate(item.gameObject, activeLevelParent);
            data.Add(new SavedInfo(saved, saved.transform.position, saved.transform.localScale, saved.transform.rotation));
            //item.gameObject.SetActive(false);
        }
        foreach(var item in bushFireflies)
        {
            item.SetActive(false);
        }
        DragAndFire dnf = DragAndFire.instance;
        dnf.rb.angularVelocity = 0;
        dnf.rb.linearVelocity = Vector2.zero;

        for (int i = 0; i < toBeRemoved.Count; i++)
        {
            for (int j = 0; j < savedData.Count; j++)
            {
                if (toBeRemoved[i] == savedData[j]) savedData.RemoveAt(j);
            }
        }
        toBeRemoved.Clear();

        //cam.enabled = true;
    }

    public void Restart()
    {
        //cam.enabled = false;
        //Camera.main.transform.position = new Vector3(0f, 0f, -10f);
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
        dnf.ActivateEffect(dnf.fireflyType);
        dnf.transform.GetChild(1).GetComponent<Tongue>().Clear();
        //if (dnf.transform.GetChild(1).childCount > 0)
        //{
        //    dnf.transform.GetChild(1).GetChild(0).TryGetComponent(out Firefly ff);
        //    if (ff != null) ff.Die();
        //}

        dnf.rb.angularVelocity = 0;
        dnf.rb.linearVelocity = Vector2.zero;
    }
    public void Clear()
    {
        foreach (var item in savedData)
        {
            Destroy(item.gameObject);
        }
        savedData.Clear();
        foreach (var item in bushFireflies)
        {
            Destroy(item);
        }
        bushFireflies.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i] == null || data[i].gameObject == null) return;
            Destroy(data[i].gameObject);
        }
        data.Clear();

        foreach (var obj in FindObjectsByType<SaveableObject>(FindObjectsSortMode.InstanceID))
        {
            Destroy(obj.gameObject);
        }
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
