using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public Color color1;
    public Color color2;
    public AnimationCurve curve;
    float i;
    public float time;
    Light2D l;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        l = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        l.color = CurveColor();
        i += Time.deltaTime;
        if (i > time)
        {
            i = 0;
        }
    }

    Color CurveColor()
    {
        return Color.Lerp(color1, color2, curve.Evaluate(i/time));
    }
}
