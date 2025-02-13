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
        if (!delayed) CustomStart();
    }
    public void CustomStart()
    {
        newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, null);
        newConnectedFirefly.TryGetComponent(out Firefly firefly);
        firefly.respawnPoint = this;
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
}
