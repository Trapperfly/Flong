using System.Collections;
using UnityEngine;

public class FireflyRespawnPoint : MonoBehaviour
{
    public GameObject fireflyPrefab;
    GameObject connectedFirefly;
    GameObject newConnectedFirefly;

    private void Start()
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
