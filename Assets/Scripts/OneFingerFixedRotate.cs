//using UnityEngine;
//using Lean.Touch;

//public class OneFingerFixedRotate : MonoBehaviour
//{
//    public float rotationSpeed = 0.2f;

//    private LeanDragTranslate leanDrag;
//    private LeanPinchScale leanPinch;

//    void Awake()
//    {
//        leanDrag = GetComponent<LeanDragTranslate>();
//        leanPinch = GetComponent<LeanPinchScale>();
//    }

//    void Update()
//    {
//        if (Input.touchCount != 1)
//        {
//            EnableLean(true);
//            return;
//        }

//        Touch touch = Input.GetTouch(0);

//        if (touch.phase == TouchPhase.Moved)
//        {
//            EnableLean(false); // disable drag & scale while rotating

//            float rotY = touch.deltaPosition.x * rotationSpeed;

//            // 🔒 Only Y axis rotation
//            transform.localRotation *= Quaternion.Euler(0f, rotY, 0f);
//        }
//    }

//    void EnableLean(bool enable)
//    {
//        if (leanDrag) leanDrag.enabled = enable;
//        if (leanPinch) leanPinch.enabled = enable;
//    }
//}


using UnityEngine;

public class OneFingerFixedRotate : MonoBehaviour
{
    public float rotationSpeed = 0.2f;

    void Update()
    {
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Moved)
        {
            float rotY = touch.deltaPosition.x * rotationSpeed;

            // Only Y axis rotation
            transform.localRotation *= Quaternion.Euler(0f, -rotY, 0f);
        }
    }
}