using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public void CustomStart()
    {
        DragAndFire.instance.transform.position = transform.position;
        Destroy(gameObject);
    }
}
