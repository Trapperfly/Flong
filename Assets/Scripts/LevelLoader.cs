using UnityEngine;
using System.Collections.Generic;
using SFB;
using static Creator;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public Transform levelParent;
    public void OpenFileDialogAndLoad()
    {
        Creator.instance.Clear();
        string directory = Application.persistentDataPath;
        var paths = StandaloneFileBrowser.OpenFilePanel("Load Level", directory, "json", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            LoadLevelFromFile(paths[0]);
        }
    }

    private void LoadLevelFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveDataWrapper data = JsonUtility.FromJson<SaveDataWrapper>(json);

        foreach (var objectData in data.savedObjects)
        {
            LoadObject(objectData);
        }

        Debug.Log("Level loaded from file: " + filePath);
    }

    private void LoadObject(ObjectData objectData)
    {
        GameObject prefab = Resources.Load<GameObject>(objectData.prefabName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {objectData.prefabName}");
            return;
        }

        GameObject obj = Instantiate(prefab, objectData.position, objectData.rotation, levelParent);
        obj.transform.localScale = objectData.scale;

        SaveableObject saveable = obj.GetComponent<SaveableObject>();
        if (saveable != null)
        {
            CustomDataBase customData = JsonUtility.FromJson<CustomDataBase>(objectData.customData);

            if (customData.type == "EnumData")
                customData = JsonUtility.FromJson<EnumData>(objectData.customData);
            else if (customData.type == "LightData")
                customData = JsonUtility.FromJson<LightData>(objectData.customData);

            saveable.LoadCustomData(customData);
        }

        Creator.instance.savedData.Add(new SavedInfo(obj, obj.transform.position, obj.transform.localScale, obj.transform.rotation));
    }
}
