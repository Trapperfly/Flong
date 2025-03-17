using UnityEngine;
using System.Collections.Generic;

public class LevelSaver : MonoBehaviour
{
    public string levelName = "Level1";

    public void SaveLevel()
    {
        LevelData levelData = new LevelData();

        foreach (var obj in FindObjectsByType<SaveableObject>(FindObjectsSortMode.InstanceID))
        {
            ObjectData objectData = new ObjectData
            {
                prefabName = obj.prefabName, // Set in SaveableObject component
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                scale = obj.transform.localScale,
                customData = obj.GetCustomData() // Implement custom data method in SaveableObject
            };

            levelData.objects.Add(objectData);
        }

        string json = JsonUtility.ToJson(levelData, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + levelName + ".json", json);

        Debug.Log($"Level saved to {Application.persistentDataPath}/{levelName}.json");
    }
}
