using UnityEngine;

public class StopGame : MonoBehaviour
{
    public Canvas canvas;
    public CameraTracker tracker;
    public Transform levelParent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Creator.instance.Restart();

            canvas.gameObject.SetActive(true);

            tracker.Lock(true);

            levelParent.gameObject.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }
}
