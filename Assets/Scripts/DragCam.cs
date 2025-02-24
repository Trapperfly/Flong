using Unity.Cinemachine;
using UnityEngine;
public class DragCam : MonoBehaviour
{
    public float speed;
    public float zoomSpeed;
    public CinemachineCamera cam;
    public CameraTracker tracker;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            tracker.transform.position -= Input.mousePositionDelta * speed * Time.deltaTime;
            Cursor.visible = false;
        }
        else
            Cursor.visible = true;
        //Camera.main.orthographicSize += Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
        cam.Lens.OrthographicSize += Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
    }
}
