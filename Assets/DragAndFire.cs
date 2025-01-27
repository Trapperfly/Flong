using System.Collections.Generic;
using Unity.Cinemachine;
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
    bool available = false;
    public bool grounded;
    public bool airial;
    public int doubleJumps = 0;
    public int aDoubleJumps = 0;
    public bool wallJumps;
    public LayerMask playerLayerMask;
    public LayerMask floorLayerMask;
    bool hitPlayer;
    public float tracePointDistance = 0.05f;
    public CameraTracker tracker;
    public GameObject dragVisuals;
    GameObject iDragVisuals;
    Transform iThumb;
    public CinemachineCamera cam;
    

    bool prediction = false;
    bool doubleJump = false;
    bool thick = false;
    bool upsideDown = false;
    bool teleport = false;
    bool sticky = false;
    bool longEyes = false;

    Vector2 groundCheckDirection = new(0, -1);

    public FireflyType fireflyType;

    public void ActivateEffect(FireflyType type)
    {
        switch (type)
        {
            case FireflyType.None:
                prediction = false;
                doubleJump = false;
                doubleJumps = 0;
                thick = false;
                rb.mass = 1;
                upsideDown = false;
                rb.gravityScale = 1f;
                groundCheckDirection = new(0, -1);
                teleport = false;
                sticky = false;
                
                longEyes = false;
                tracker.adjust = 0.1f;
                break;
            case FireflyType.Prediction:
                prediction = true;
                break;
            case FireflyType.DoubleJump:
                doubleJump = true;
                doubleJumps = 1;
                break;
            case FireflyType.Thick:
                thick = true;
                rb.mass = 2;
                rb.gravityScale = 2;
                break;
            case FireflyType.UpsideDown:
                upsideDown =true;
                rb.gravityScale = -1f;
                groundCheckDirection = new(0, 1);
                break;
            case FireflyType.Teleport:
                teleport = true;
                break;
            case FireflyType.Sticky:
                sticky = true;
                break;
            case FireflyType.LongEyes:
                tracker.adjust = 0.5f;
                break;
            default:
                break;
        }
    }
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
        if (Input.GetMouseButtonDown(0)) { 
            if (grounded || (airial && aDoubleJumps > 0))
                holding = true;
        }
        available = (grounded && holding) || (airial && aDoubleJumps > 0 && holding);
        if (Input.GetMouseButtonDown(0))
        {
            if (available)
                OnClick();
        }
            //if (!hitPlayer) { holding = false; return; }
        if (available)
        {
            Drag();
            if (force > maxForce * 0.1f) force = maxForce * 0.1f;
            CalculateTrajectory();
        }
        if ( available && Input.GetMouseButtonUp(0) ) 
        {
            if (teleport) Teleport();
            else Launch();
            if (airial) { aDoubleJumps--; }
            holding = false;
            //hitPlayer = false;
        }
        if (Input.GetMouseButtonUp(0)) {
            holding = false;
            //hitPlayer = false;
        }
        GroundCheck();
        //grounded = false;
        if (!holding) { cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, 5, 0.02f); }
    }
    void GroundCheck()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, groundCheckDirection, 0.3f, floorLayerMask);
        if (hits.Length > 0)
        {
            grounded = true;
            airial = false;
            aDoubleJumps = doubleJumps;
        }
        else
        {
            grounded = false;
            airial = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Floor"))
        {
            if (sticky)
            {
                rb.gravityScale = 0;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                aDoubleJumps++;
            }
        }
    }
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Floor"))
    //    {
    //        if (!airial)
    //        grounded = true;
    //        airial = false;
    //    }
    //}

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
        cam.Lens.OrthographicSize = Mathf.Lerp(5, 10, force / maxForce);
    }
    void CalculateTrajectory()
    {
        if (!prediction) return;
        lr.enabled = true;
        for (int i = 0; i < lr.positionCount; i++)
        {
            Vector2 pos = (Vector2)transform.position + (direction * force * forceMultiplier * (i * tracePointDistance));
            Vector2 posWithGrav = pos - new Vector2(0, i * i * (tracePointDistance * tracePointDistance * 4.9175f));
            lr.SetPosition(i, posWithGrav);
        }
    }
    void Teleport()
    {
        lr.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 200, floorLayerMask);
        if (hit)
        {
            transform.position = hit.point;
            grounded = false;
        }
        tracker.locked = false;
        Destroy(iDragVisuals);
    }
    void Launch()
    {
        if (sticky) rb.gravityScale = 1;
        lr.enabled = false;
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
