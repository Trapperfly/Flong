using UnityEngine;

public class Pathing : MonoBehaviour
{
    LineRenderer follow;
    public float speed;
    float aSpeed;
    float timer;
    int t1, t2;
    bool backwards;
    public bool looping;
    public Gradient gradient = new();

    private void Start()
    {
        
        follow = GetComponent<LineRenderer>();
        t1 = 0;
        t2 = 1;
        if (looping)
        {
            follow.SetPosition(follow.positionCount - 1, follow.GetPosition(0));
        }
        follow.colorGradient = gradient;
        for (int i = 0; i < gradient.alphaKeys.Length; i++) gradient.alphaKeys[i].alpha = 0f;
    }
    private void Update()
    {
        transform.position = Vector2.Lerp(follow.GetPosition(t1), follow.GetPosition(t2), timer);
        aSpeed = (Time.deltaTime * speed) / Vector2.Distance(follow.GetPosition(t1), follow.GetPosition(t2));
        if (timer > 1 || timer < 0)
        {
            if (!backwards && follow.positionCount > t2 + 1)
            {
                t1++;
                t2++;
            }
            else if (backwards && t1 > 0)
            {
                t1--;
                t2--;
            }
            else if (looping)
            {
                t1 = 0;
                t2 = 1;
            }
            else backwards = !backwards;
            timer = backwards ? 1 : 0;
        }
        timer += backwards ? -aSpeed : aSpeed;
        //timer += Time.deltaTime;
    }
}
