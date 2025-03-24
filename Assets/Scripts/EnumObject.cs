using UnityEngine;

[System.Serializable]
public class EnumObject : SaveableObject
{
    public FireflyType state = FireflyType.None;

    private void Start()
    {
        state = GetComponent<FireflyRespawnPoint>().type;
    }

    public override CustomDataBase GetCustomData()
    {
        return new EnumData(state);
    }

    public override void LoadCustomData(CustomDataBase data)
    {
        if (data is EnumData enumData)
        {
            state = enumData.state;
            GetComponent<FireflyRespawnPoint>().type = state;
        }
    }
}
