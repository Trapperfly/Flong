using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public float adjust;
    public Transform player;
    public bool locked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DragAndFire.instance.disabled) return;
        if (locked) return;
        transform.position = Vector2.Lerp((Vector2)player.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), adjust);
    }
}
