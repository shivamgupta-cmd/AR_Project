using UnityEngine;
using Vuforia;
using UnityEngine.Events;

public class ARTouchEvent : MonoBehaviour
{
    private Camera mainCamera;
    public UnityEvent Ontouchevents;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void HandleTouch()
    {
        // Check for touch input on mobile or mouse input in the editor
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // You can check for specific tags or conditions here if needed
                if (hit.transform.CompareTag("ScannedObject"))
                {
                    Ontouchevents.Invoke();
                    Debug.Log("Scanned object touched!");
                    // Trigger your event here
                }
            }
        }
    }
    public void ChangeColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
    }
}