using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragAndFire : MonoBehaviour
{
    Rigidbody2D rb;
    LineRenderer lr;
    bool holding = false;
    Vector2 direction;
    float force;
    Vector2 mousePos;
    public float forceMultiplier;
    bool grounded;
    public LayerMask playerLayerMask;
    bool hitPlayer;
    public float tracePointDistance = 0.05f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) { holding = true; OnClick(); }
        if (!hitPlayer) { holding = false; return; }
        if ( grounded && holding )
        {
            Drag();
            CalculateTrajectory();
        }
        if ( grounded && holding && Input.GetMouseButtonUp(0)) 
        {
            Launch();
            holding = false;
            hitPlayer = false;
        }
        if (Input.GetMouseButtonUp(0)) {
            holding = false;
            hitPlayer = false;
        }
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 200, playerLayerMask );

        List<GameObject> hitsList = new();

        foreach (RaycastHit2D hit in hits)
        {
            hitsList.Add( hit.transform.gameObject );
        }

        if (hitsList.Contains(gameObject))
        {
            hitPlayer = true;
        }
    }

    //void OnClickUp()
    //{

    //}
    void Drag()
    {
        force = Vector2.Distance(mousePos, transform.position);
        direction = -(mousePos - (Vector2)transform.position).normalized;
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
    }
}
//0.05 -> 0.25
//0.1 -> 0.5
//0.5 -> 2.5
