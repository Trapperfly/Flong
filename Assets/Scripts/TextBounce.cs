using System.Collections;
using UnityEngine;

public class TextBounce : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BounceAndDie());
    }

    IEnumerator BounceAndDie()
    {
        float timer = 0f;
        Vector2 endpos = transform.position + new Vector3(0,0.6f);
        while (true)
        {
            transform.position = Vector2.Lerp(transform.position, endpos, timer);
            timer += Time.deltaTime * 0.5f;
            yield return null;
            if (timer > 1)
                break;
        }
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
