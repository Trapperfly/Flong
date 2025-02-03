using System.Collections;
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
    public Sprite sqSprite;
    public Sprite ciSprite;

    public GameObject fireballPrefab;

    public float flapForce;
    public float flapCooldown;
    float flapTimer = 0;
    float highestPoint;
    

    bool prediction = false;
    bool doubleJump = false;
    bool thick = false;
    bool upsideDown = false;
    bool teleport = false;
    bool sticky = false;
    bool longEyes = false;
    bool fireball = false;
    bool flappy = false;
    bool phase = false;

    public float phaseTime;
    public bool phaseAvailable;
    bool phasing;
    public LayerMask phasingLayerMask;
    public LayerMask normalLayerMask;

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
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = true;
                GetComponent<SpriteRenderer>().sprite = sqSprite;

                fireball = false;

                flappy = false;

                phase = false;
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
                upsideDown = true;
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
            case FireflyType.Flappy:
                flappy = true;
                break;
            case FireflyType.Spicy:
                fireball = true;
                break;
            case FireflyType.Round:
                GetComponent<CircleCollider2D>().enabled = true;
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().sprite = ciSprite;
                break;
            case FireflyType.Phase:
                phase = true;
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
            if ((grounded || fireball) || (airial && aDoubleJumps > 0))
                holding = true;
            else if (flappy && flapTimer > flapCooldown) Flap();
            else if (phase && phaseAvailable)
                StartCoroutine(Phase());
        }
        available = ((grounded || fireball) && holding) || (airial && aDoubleJumps > 0 && holding);
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
            else if (fireball) Fireball();
            else Launch();
            if (airial) { aDoubleJumps--; }
            holding = false;
            //hitPlayer = false;
        }
        if (Input.GetMouseButtonUp(0)) {
            holding = false;
            //hitPlayer = false;
        }
        if (!phasing)
            GroundCheck();
        //grounded = false;
        if (!holding) { cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, 5, 0.02f); }

        flapTimer += Time.deltaTime;
        if (transform.position.y > highestPoint)
            highestPoint = transform.position.y;
        if (grounded) 
        {
            highestPoint = transform.position.y;
            phaseAvailable = true;
        }

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
        if (!fireball) cam.Lens.OrthographicSize = Mathf.Lerp(5, 10, force / maxForce);
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
    void Fireball()
    {
        Rigidbody2D frb = Instantiate(fireballPrefab, transform.position, Quaternion.identity, null).GetComponent<Rigidbody2D>();
        frb.AddForce(direction * force * forceMultiplier, ForceMode2D.Impulse);
        tracker.locked = false;
        Destroy(iDragVisuals);
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

    void Flap()
    {
        rb.linearVelocityY = 0;
        rb.AddForce(flapForce * Mathf.Lerp(0, 1, Vector2.Distance(new Vector2(transform.position.x, highestPoint), transform.position) / 5) * Vector2.up, ForceMode2D.Impulse);
        flapTimer = 0;
    }

    IEnumerator Phase()
    {
        BoxCollider2D _bc = GetComponent<BoxCollider2D>();
        SpriteRenderer _sr = GetComponent<SpriteRenderer>();
        phaseAvailable = false;
        phasing = true;
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.3f);
        //_bc.enabled = false;
        Physics2D.SetLayerCollisionMask(6, phasingLayerMask); //Player layer
        yield return new WaitForSeconds(phaseTime);
        Physics2D.SetLayerCollisionMask(6, normalLayerMask); //Player layer
        //_bc.enabled = true;
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1f);
        phasing = false;
        yield return null;
    }
}
//0.05 -> 0.25
//0.1 -> 0.5
//0.5 -> 2.5
