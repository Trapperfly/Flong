using System.Collections.Generic;
using UnityEngine;

public class SaveableObject : MonoBehaviour
{
    public string prefabName; // This should match the prefab name used for instantiating

    public virtual string GetCustomData()
    {
        // Override this in child classes if you need custom data (e.g., health, states)
        return "{}";
    }
}

[System.Serializable]
public class LevelData
{
    public List<ObjectData> objects = new();
}

[System.Serializable]
public class ObjectData
{
    public string prefabName; // Or unique ID
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string customData; // For any additional data (e.g., JSON string or key-value pairs)
}
