
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Breakable : MonoBehaviour
{
    List<ShadowCaster2D> scs = new();
    List<Collider2D> colliders = new();
    List<Rigidbody2D> rbs = new();
    Transform parent;
    public float breakforce;
    public float neededForce;

    private void Start()
    {
        int i = 0;
        parent = transform.parent;
        foreach (Transform child in parent)
        {
            if (i != 0)
            {
                scs.Add(child.GetComponent<ShadowCaster2D>());
                colliders.Add(child.GetComponent<Collider2D>());
                rbs.Add(child.GetComponent<Rigidbody2D>());
            }
            i++;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something hit floor");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit floor");
            if (collision.GetComponent<DragAndFire>().fireflyType == FireflyType.Thick)
            {
                Debug.Log("Player was thick. Hit the collider at " + collision.GetComponent<Rigidbody2D>().linearVelocity.y + " downward force");
                if (collision.GetComponent<Rigidbody2D>().linearVelocity.y < -neededForce)
                {
                    Debug.Log("Player had enough velocity");
                    Break(collision);
                }
            }
        }
    }
    void Break(Collider2D col)
    {
        parent.GetComponent<Collider2D>().enabled = false;
        foreach(ShadowCaster2D sc in scs)
        {
            sc.enabled = true;
        }
        foreach(Collider2D collider in colliders)
        {
            collider.enabled = true;
        }
        foreach (Rigidbody2D rb in rbs)
        {
            rb.simulated = true;
            rb.AddForce((rb.transform.position - col.transform.position).normalized * (breakforce / Vector2.Distance(rb.transform.position, col.transform.position)), ForceMode2D.Impulse);
            rb.transform.parent = Creator.instance.debrisParent;
        }
        Destroy(parent.gameObject);
    }
}
