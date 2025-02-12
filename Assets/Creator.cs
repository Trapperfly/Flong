using System.Collections.Generic;
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


    public Create create;

    public List<GameObject> dragObjects = new();

    Vector2 worldMousePos;

    Transform t;
    Vector2 startPos;
    void Update()
    {
        worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        switch (create)
        {
            case Create.Nothing:
                break;
            case Create.Floor:
                if (Input.GetMouseButtonDown(0)) 
                {
                    t = Instantiate(dragObjects[0], worldMousePos, Quaternion.identity, null).transform;
                    startPos = worldMousePos;
                }
                if (Input.GetMouseButton(0))
                {
                    t.position = Vector2.Lerp(startPos, worldMousePos, 0.5f);
                    t.localScale = new Vector2
                        (Vector2.Distance
                        (startPos, new Vector2(worldMousePos.x, startPos.y)), 
                        Vector2.Distance
                        (startPos, new Vector2(startPos.x, worldMousePos.y)));
                }
                break;
            case Create.AbsoluteFloor:
                if (Input.GetMouseButtonDown(0))
                {
                    t = Instantiate(dragObjects[1], worldMousePos, Quaternion.identity, null).transform;
                    startPos = worldMousePos;
                }
                if (Input.GetMouseButton(0))
                {
                    t.position = Vector2.Lerp(startPos, worldMousePos, 0.5f);
                    t.localScale = new Vector2
                        (Vector2.Distance
                        (startPos, new Vector2(worldMousePos.x, startPos.y)),
                        Vector2.Distance
                        (startPos, new Vector2(startPos.x, worldMousePos.y)));
                }
                break;
            default:
                break;
        }
    }
}
public enum Create
{
    Nothing,
    Move,
    Scale,
    Rotate,
    Delete,
    Floor,
    AbsoluteFloor,

}
