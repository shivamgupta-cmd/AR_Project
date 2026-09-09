using UnityEngine;
using Lean.Touch;

public class LeanModelController : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public float scaleSpeed = 0.01f;
    public float heightSpeed = 0.01f;

    void Update()
    {
        // ROTATE
        if (LeanTouch.Fingers.Count == 1)
        {
            var finger = LeanTouch.Fingers[0];
            transform.Rotate(0, -finger.ScreenDelta.x * rotateSpeed, 0);
        }

        // SCALE
        float pinch = LeanGesture.GetPinchScale(LeanTouch.Fingers);
        if (pinch != 1)
        {
            transform.localScale *= pinch;
        }

        // HEIGHT MOVE (2 finger drag)
        if (LeanTouch.Fingers.Count == 2)
        {
            float moveY = LeanGesture.GetScreenDelta(LeanTouch.Fingers).y;
            transform.Translate(0, moveY * heightSpeed, 0);
        }
    }
}
