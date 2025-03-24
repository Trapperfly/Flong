using System.Collections.Generic;
using UnityEngine;
using static Creator;

public class SaveableObject : MonoBehaviour
{
    public string prefabName; // This should match the prefab name used for instantiating
    public virtual CustomDataBase GetCustomData()
    {
        // Override this in child classes if you need custom data (e.g., health, states)
        return new CustomDataBase{ type = "None" };
    }
    public virtual void LoadCustomData(CustomDataBase data)
    {
        // Implement in subclasses
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
    public ObjectData(string prefabName, Vector2 pos, Vector3 scale, Quaternion rotation, CustomDataBase customData)
    {
        this.prefabName = prefabName;
        position = pos;
        this.scale = scale;
        this.rotation = rotation;
        this.customData = JsonUtility.ToJson(customData); // Convert custom data to JSON
    }
}

[System.Serializable]
public class CustomDataBase
{
    public string type; // Stores the type of data (e.g., "EnumData", "LightData")
}
[System.Serializable]
public class EnumData : CustomDataBase
{
    public FireflyType state;

    public EnumData(FireflyType state)
    {
        type = "EnumData";
        this.state = state;
    }
}
[System.Serializable]
public class LightData : CustomDataBase
{
    public float range;
    public float shadowIntensity;

    public LightData(float range, float shadowIntensity)
    {
        type = "LightData";
        this.range = range;
        this.shadowIntensity = shadowIntensity;
    }
}

[System.Serializable]
public class SaveDataWrapper
{
    public List<ObjectData> savedObjects;

    public SaveDataWrapper(List<ObjectData> savedObjects)
    {
        this.savedObjects = savedObjects;
    }
}