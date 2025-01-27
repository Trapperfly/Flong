using UnityEngine;

public class Firefly : MonoBehaviour
{
    public FireflyType FireflyType;
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
}
