using System.Collections;
using UnityEngine;

public class FireflyRespawnPoint : MonoBehaviour
{
    public GameObject fireflyPrefab;
    GameObject connectedFirefly;
    GameObject newConnectedFirefly;
    public bool delayed = false;

    private void Start()
    {
        if (delayed) { }
        else
        {
            newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, null);
            newConnectedFirefly.TryGetComponent(out Firefly firefly);
            firefly.respawnPoint = this;
        }
    }
    public void CustomStart()
    {
        newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, null);
        newConnectedFirefly.TryGetComponent(out Firefly firefly);
        firefly.respawnPoint = this;
        Creator.instance.bushFireflies.Add(firefly.gameObject);
    }

    public IEnumerator Picked()
    {
        if (connectedFirefly != null) { StartCoroutine(connectedFirefly.GetComponent<Firefly>().Die()); }
        connectedFirefly = newConnectedFirefly;
        yield return new WaitForSeconds(3);
        newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, null);
        newConnectedFirefly.TryGetComponent(out Firefly firefly);
        firefly.respawnPoint = this;
    }

    private void OnDestroy()
    {
        if (connectedFirefly != null) { Destroy(connectedFirefly.gameObject); }
        if (newConnectedFirefly != null) { Destroy(newConnectedFirefly.gameObject); }
    }
}
