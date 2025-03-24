using UnityEngine;
using System.Collections.Generic;
using static Creator;

public class LevelSaver : MonoBehaviour
{
    public TMPro.TMP_InputField input;

    public void SaveLevel()
    {

        List<ObjectData> objectList = new();

        foreach (var obj in FindObjectsByType<SaveableObject>(FindObjectsSortMode.InstanceID))
        {
            objectList.Add(new ObjectData(
                obj.prefabName,
                obj.transform.position,
                obj.transform.localScale,
                obj.transform.rotation,
                obj.GetCustomData() // Convert custom data to JSON
            ));
        }

        string levelName;
        if (input.text == null || input.text.Length == 0)
        {
            levelName = "FallbackLevelName";
        } else
        {
            levelName = input.text;
        }

        string json = JsonUtility.ToJson(new SaveDataWrapper(objectList), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + levelName + ".json", json);

        Debug.Log($"Level saved to {Application.persistentDataPath}/{levelName}.json");
    }
}
