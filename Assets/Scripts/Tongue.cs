using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tongue : MonoBehaviour
{
    LineRenderer lr;
    public LayerMask LayerMask;
    //public float distance;
    public Vector2 hitPoint;
    bool anim;
    public float time;
    float i;
    public Transform carried;
    public Transform placing;
    public ShadowCaster2D sc;
    public DragAndFire dnf;

    public Vector2 storedScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DragAndFire.instance.disabled) return;
        if (!anim && Input.GetMouseButtonDown(1))
        {
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask);
            if (hit)
            {

                if (hit.transform.gameObject.layer == 7 || hit.transform.gameObject.layer == 10)// floor 
                {
                    hitPoint = hit.point;
                }
                else
                {
                    carried = hit.transform;
                    hitPoint = hit.transform.position;

                }
            }
            else
            {
                hitPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            StartCoroutine(nameof(TongueAnim));
        }
        lr.SetPosition(0, transform.position);
        if (!anim) lr.SetPosition(1, transform.position);
    }
    IEnumerator TongueAnim()
    {
        Transform rbObject = null;
        anim = true;
        if (transform.childCount > 0)
        {
            sc.castingOption = ShadowCaster2D.ShadowCastingOptions.CastShadow;
            placing = transform.GetChild(0);
            transform.GetChild(0).SetParent(null);
            placing.TryGetComponent(out Rigidbody2D rb);
            if (rb != null)
            {
                //rb.TryGetComponent(out SpriteRenderer renderer);
                //renderer.enabled = true;
                rb.simulated = true;
                rbObject = placing;
                placing.TryGetComponent(out ShadowCaster2D sco);
                if (sco != null)
                {
                    sco.castingOption = ShadowCaster2D.ShadowCastingOptions.CastShadow;
                }
                placing.TryGetComponent(out Collider2D col);
                if (col != null)
                {
                    col.enabled = true;
                }
            }
            
        }
        yield return null;
        while (i < time)
        {
            i += Time.deltaTime;
            if (rbObject != null) { rbObject.localScale = Vector2.Lerp(Vector2.zero, storedScale, i / time); }
            lr.SetPosition(1, Vector2.Lerp(transform.position, hitPoint, i / time));
            if (placing) placing.position = lr.GetPosition(1);
            yield return null;
        }
        if (placing) { 
            placing.GetComponent<Collider2D>().enabled = true;
            placing.TryGetComponent(out Firefly firefly);
            if (firefly != null)
            {
                dnf.fireflyType = FireflyType.None;
                dnf.ActivateEffect(dnf.fireflyType);
                firefly.GetComponent<SpriteRenderer>().enabled = true;
            }
            
            placing = null;
            rbObject = null;

        }
        
        if (carried)
        {
            carried.TryGetComponent(out Rigidbody2D rb);
            if (rb != null)
            {
                //rb.TryGetComponent(out SpriteRenderer renderer);
                //renderer.enabled = true;
                rb.simulated = true;
                rbObject = carried;
                storedScale = rbObject.transform.localScale;
                carried.TryGetComponent(out Collider2D col);
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }
        while (i > 0)
        {
            i -= Time.deltaTime;
            if (rbObject != null) { rbObject.localScale = Vector2.Lerp(Vector2.zero, storedScale, i / time); }
            lr.SetPosition(1, Vector2.Lerp(transform.position, hitPoint, i / time));
            if (carried) carried.position = lr.GetPosition(1);
            yield return null;
        }
        if (carried)
        {
            if (rbObject == null)
                sc.castingOption = ShadowCaster2D.ShadowCastingOptions.NoShadow;
            carried.SetParent(transform);
            carried.localPosition = Vector3.zero;
            carried.GetComponent<Collider2D>().enabled = false;
            carried.TryGetComponent(out Firefly firefly);
            if (firefly != null)
            {
                firefly.GetComponent<SpriteRenderer>().enabled = false;
                dnf.fireflyType = firefly.fireflyType;
                dnf.ActivateEffect(dnf.fireflyType);
                dnf.SpawnText(firefly.colors[(int)firefly.fireflyType], firefly.fireflyType);
                firefly.Picked();
            }
            carried.TryGetComponent(out Rigidbody2D rb);
            if (rb != null)
            {
                rb.simulated = false;
                //rb.TryGetComponent(out SpriteRenderer renderer);
                //renderer.enabled = false;
                carried.TryGetComponent(out ShadowCaster2D sco);
                if (sco != null)
                {
                    sco.castingOption = ShadowCaster2D.ShadowCastingOptions.NoShadow;
                }
            }
            
        }
        anim = false;
        carried = null;
        yield return null;
    }
}
