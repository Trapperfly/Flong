using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowCasterHelper : MonoBehaviour
{
    ShadowCaster2D shadowCaster;
    void Start()
    {
        shadowCaster = GetComponent<ShadowCaster2D>();
        // Use reflection to access the private field for Casting Source
        var type = typeof(ShadowCaster2D);
        var field = type.GetField("m_CastsShadows", BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            // Toggle shadow casting off and on
            field.SetValue(shadowCaster, false);
            field.SetValue(shadowCaster, true);
        }
    }
}
