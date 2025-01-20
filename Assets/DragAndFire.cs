using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragAndFire : MonoBehaviour
{
    Rigidbody2D rb;
    LineRenderer lr;
    Vector2 clickPos;
    bool holding = false;
    Vector2 direction;
    float force;
    public float maxForce;
    Vector2 mousePos;
    Vector2 worldMousePos;
    public float forceMultiplier;
    bool grounded;
    public LayerMask playerLayerMask;
    bool hitPlayer;
    public float tracePointDistance = 0.05f;
    public CameraTracker tracker;
    public GameObject dragVisuals;
    GameObject iDragVisuals;
    Transform iThumb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) { holding = true; OnClick(); }
        //if (!hitPlayer) { holding = false; return; }
        if ( grounded && holding )
        {
            Drag();
            if (force > maxForce * 0.1f) force = maxForce * 0.1f;
            CalculateTrajectory();
        }
        if ( grounded && holding && Input.GetMouseButtonUp(0)) 
        {
            Launch();
            holding = false;
            //hitPlayer = false;
        }
        if (Input.GetMouseButtonUp(0)) {
            holding = false;
            //hitPlayer = false;
        }
        //grounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            grounded = true;
        }
    }

    void OnClick()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 200, playerLayerMask );

        //List<GameObject> hitsList = new();

        //foreach (RaycastHit2D hit in hits)
        //{
        //    hitsList.Add( hit.transform.gameObject );
        //}

        //if (hitsList.Contains(gameObject))
        //{
        //    hitPlayer = true;
        //}
        
        tracker.locked = true;
        clickPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        iDragVisuals = Instantiate(dragVisuals, new Vector3(worldMousePos.x, worldMousePos.y, 0), Quaternion.identity, null);
        iThumb = iDragVisuals.transform.GetChild(0);
    }

    //void OnClickUp()
    //{

    //}
    void Drag()
    {
        force = Vector2.Distance(mousePos, clickPos);
        direction = -(mousePos - clickPos).normalized;
        iThumb.position = new Vector3(worldMousePos.x, worldMousePos.y, 0);
    }
    void CalculateTrajectory()
    {
        lr.enabled = true;
        for (int i = 0; i < lr.positionCount; i++)
        {
            Vector2 pos = (Vector2)transform.position + (direction * force * forceMultiplier * (i * tracePointDistance));
            Vector2 posWithGrav = pos - new Vector2(0, i * i * (tracePointDistance * tracePointDistance * 4.9175f));
            lr.SetPosition(i, posWithGrav);
        }
    }

    void Launch()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force * forceMultiplier, ForceMode2D.Impulse);
        grounded = false;
        tracker.locked = false;
        Destroy(iDragVisuals);
    }
}
//0.05 -> 0.25
//0.1 -> 0.5
//0.5 -> 2.5
