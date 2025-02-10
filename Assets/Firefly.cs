using System.Collections;
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
    public FireflyRespawnPoint respawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Spawn());
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

    public void Picked()
    {
        if (respawnPoint != null)
        {
            StartCoroutine(respawnPoint.Picked());
            respawnPoint = null;
        }
    }

    IEnumerator Spawn()
    {
        float timer = 0;
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;

        TryGetComponent(out Light2D light);
        float intensity = light.intensity;
        light.intensity = 0;
        while (true)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, scale, timer);
            light.intensity = Mathf.Lerp(0, intensity, timer);
            yield return null;

            if (timer >= 1) 
            {
                light.intensity = intensity;
                transform.localScale = scale; 
                break;
            }
        }
    }

    public IEnumerator Die()
    {
        float timer = 0;
        Vector3 scale = transform.localScale;

        TryGetComponent(out Light2D light);
        float intensity = light.intensity;
        while (true)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(scale, Vector3.zero, timer);
            light.intensity = Mathf.Lerp(intensity, 0, timer);
            yield return null;

            if (timer > 1)
            {
                break; 
            }
        }
        Destroy(gameObject);
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
    Phase,
    Shatter,
}
