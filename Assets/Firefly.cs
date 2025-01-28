using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Firefly : MonoBehaviour
{
    public FireflyType fireflyType;
    public List<Color> colors = new();
    Color color1;
    Color color2;
    public AnimationCurve curve;
    float i;
    float j;
    public float time;
    Light2D l;
    float value;
    Material material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        i += Random.Range(-5000, 5000);
        j += Random.Range(-5000, 5000);
        l = GetComponent<Light2D>();
        color1 = colors[(int)fireflyType];
        color2 = color1 * 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        value = Mathf.PerlinNoise1D(i);
        material.SetVector("_Pos", new Vector2(Mathf.PerlinNoise1D(i) - 0.5f, Mathf.PerlinNoise1D(j) - 0.5f) * 0.5f);
        l.color = CurveColor(value);
        i += Time.deltaTime * time;
        j += Time.deltaTime * time;
        //if (i > time)
        //{
        //    i = 0;
        //}
    }

    Color CurveColor(float value)
    {
        return Color.Lerp(color1, color2, curve.Evaluate(value));
    }
}

public enum FireflyType
{
    None,
    Prediction,
    DoubleJump,
    Thick,
    UpsideDown,
    Teleport,
    Sticky,
    LongEyes,
    Flappy,
    Spicy,
    Round,
}
