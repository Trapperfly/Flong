using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class LightObject : SaveableObject
{
    public Light2D lightComponent;

    public override CustomDataBase GetCustomData()
    {
        return new LightData(lightComponent.pointLightOuterRadius, lightComponent.shadowIntensity);
    }

    public override void LoadCustomData(CustomDataBase data)
    {
        if (data is LightData lightData)
        {
            lightComponent.pointLightOuterRadius = lightData.range;
            lightComponent.shadowIntensity = lightData.shadowIntensity;
        }
    }
}
