using UnityEngine;

public class DragVisualsLine : MonoBehaviour
{
    LineRenderer lr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(1, transform.position);
        lr.SetPosition(0, transform.GetChild(0).position);
    }
}
