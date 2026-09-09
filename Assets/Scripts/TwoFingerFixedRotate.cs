

using UnityEngine;
using Lean.Touch;

public class TwoFingerFixedRotate : MonoBehaviour
{
    public float rotationSpeed = 0.12f;

    private Vector2 lastMid;
    private bool rotating;

    private LeanDragTranslate leanDrag;
    private LeanPinchScale leanPinch;

    void Awake()
    {
        leanDrag = GetComponent<LeanDragTranslate>();
        leanPinch = GetComponent<LeanPinchScale>();
    }

    void Update()
    {
        if (Input.touchCount != 2)
        {
            if (rotating)
            {
                rotating = false;
                EnableLean(true);
            }
            return;
        }

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        float currDist = Vector2.Distance(t0.position, t1.position);
        float prevDist = Vector2.Distance(
            t0.position - t0.deltaPosition,
            t1.position - t1.deltaPosition
        );

        float pinchDelta = Mathf.Abs(currDist - prevDist);

        // 👉 If pinch happening → don't rotate, keep Lean ON
        if (pinchDelta > 3f)
        {
            if (rotating)
            {
                rotating = false;
                EnableLean(true);
            }
            return;
        }

        Vector2 mid = (t0.position + t1.position) * 0.5f;

        if (!rotating)
        {
            rotating = true;
            lastMid = mid;
            EnableLean(false); // disable drag & scale during rotate
            return;
        }

        Vector2 delta = mid - lastMid;

        // 🔒 ONLY Y-AXIS ROTATION
        float rotY = delta.x * rotationSpeed;

        transform.localRotation *= Quaternion.Euler(0f, rotY, 0f);

        lastMid = mid;
    }

    void EnableLean(bool enable)
    {
        if (leanDrag) leanDrag.enabled = enable;
        if (leanPinch) leanPinch.enabled = enable;
    }
}
