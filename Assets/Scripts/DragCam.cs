using UnityEngine;
public class DragCam : MonoBehaviour
{
    public float speed;
    public float zoomSpeed;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            transform.position -= Input.mousePositionDelta * speed * Time.deltaTime;
            Cursor.visible = false;
        }
        else
            Cursor.visible = true;
        Camera.main.orthographicSize += Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
    }
}
