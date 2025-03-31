using System.Collections;
using UnityEngine;

public class FireflyRespawnPoint : MonoBehaviour
{
    public FireflyType type;
    public GameObject fireflyPrefab;
    GameObject connectedFirefly;
    GameObject newConnectedFirefly;
    public bool delayed = false;

    private void Start()
    {
        string name = "Fireflies/FF_";
        name += type;
        Debug.Log(name);
        fireflyPrefab = Resources.Load<GameObject>(name);
        if (fireflyPrefab == null) { Debug.Log("Not found"); }
        //fireflyPrefab = Creator.instance.fireflyPrefabs[(int)type];
        if (delayed) { }
        else
        {
            
            Resources.Load<GameObject>("");
            if (Creator.instance.levelParent.gameObject.activeSelf)
                newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, Creator.instance.levelParent);
            else
                newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, Creator.instance.activeLevelParent);
            newConnectedFirefly.TryGetComponent(out Firefly firefly);
            firefly.respawnPoint = this;
        }
    }
    public void CustomStart()
    {
        if (Creator.instance.levelParent.gameObject.activeSelf)
            newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, Creator.instance.levelParent);
        else
            newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, Creator.instance.activeLevelParent);
        newConnectedFirefly.TryGetComponent(out Firefly firefly);
        firefly.respawnPoint = this;
        Creator.instance.bushFireflies.Add(firefly.gameObject);
    }

    public IEnumerator Picked()
    {
        if (connectedFirefly != null) { StartCoroutine(connectedFirefly.GetComponent<Firefly>().Die()); }
        connectedFirefly = newConnectedFirefly;
        yield return new WaitForSeconds(3);
        newConnectedFirefly = Instantiate(fireflyPrefab, transform.position, Quaternion.identity, Creator.instance.activeLevelParent);
        newConnectedFirefly.TryGetComponent(out Firefly firefly);
        firefly.respawnPoint = this;
    }

    private void OnDestroy()
    {
        if (connectedFirefly != null) { Destroy(connectedFirefly.gameObject); }
        if (newConnectedFirefly != null) { Destroy(newConnectedFirefly.gameObject); }
    }
}
