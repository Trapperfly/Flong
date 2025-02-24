using UnityEngine;

public class LeafLine : MonoBehaviour
{
    FixedJoint2D joint;
    LineRenderer lr;
    Transform tracker;

    private void Start()
    {
        joint = GetComponent<FixedJoint2D>();
        lr = GetComponent<LineRenderer>();
        tracker = transform.GetChild(0);
    }

    private void Update()
    {
        lr.SetPosition(0, joint.connectedAnchor);
        lr.SetPosition(1, tracker.position);
    }
}
