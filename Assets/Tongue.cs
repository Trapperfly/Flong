using System.Collections;
using UnityEngine;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim && Input.GetMouseButtonDown(1))
        {
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask);
            if (hit)
            {
                hitPoint = hit.point;
                if (hit.transform.gameObject.layer != 7) // floor
                    carried = hit.transform;
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
        anim = true;
        if (transform.childCount > 0)
        {
            placing = transform.GetChild(0);
            transform.GetChild(0).SetParent(null);
        }
        yield return null;
        while (i < time)
        {
            i += Time.deltaTime;
            lr.SetPosition(1, Vector2.Lerp(transform.position, hitPoint, i / time));
            if (placing) placing.position = lr.GetPosition(1);
            yield return null;
        }
        if (placing) { 
            placing.GetComponent<Collider2D>().enabled = true; 
            placing = null;
        }
        while (i > 0)
        {
            i -= Time.deltaTime;
            lr.SetPosition(1, Vector2.Lerp(transform.position, hitPoint, i / time));
            if (carried) carried.position = lr.GetPosition(1);
            yield return null;
        }
        if (carried)
        {
            carried.SetParent(transform);
            carried.localPosition = Vector3.zero;
            carried.GetComponent<Collider2D>().enabled = false;
        }
        anim = false;
        carried = null;
        yield return null;
    }
}
