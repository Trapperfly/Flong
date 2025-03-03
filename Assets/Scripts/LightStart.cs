using UnityEngine;

public class LightStart : MonoBehaviour
{
    public bool delayedStart = false;

    private void Start()
    {
        if (!delayedStart)
            CustomStart();
    }
    public void CustomStart()
    {
        Destroy(gameObject.GetComponent<LightStart>());
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        Destroy(gameObject.GetComponent<Collider2D>());
    }
}
