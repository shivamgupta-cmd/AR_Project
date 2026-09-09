using UnityEngine;
using System.Collections.Generic;

public class TouchEventManager : MonoBehaviour
{
    private List<ARTouchEvent> touchEventObjects = new List<ARTouchEvent>();
    private bool isTouchEventActive = false;  // Default to false, so touch events are disabled initially

    void Start()
    {
        // Find all GameObjects with the ARTouchEvent component
        ARTouchEvent[] objectsWithTouchEvent = FindObjectsOfType<ARTouchEvent>();
        foreach (ARTouchEvent obj in objectsWithTouchEvent)
        {
            touchEventObjects.Add(obj);
        }

        // Ensure touch events are initially disabled
        isTouchEventActive = false;
    }

    void Update()
    {
        if (isTouchEventActive)
        {
            foreach (ARTouchEvent touchEvent in touchEventObjects)
            {
                touchEvent.HandleTouch();
            }
        }
    }

    public void EnableTouchEvents()
    {
        isTouchEventActive = true;
        Debug.Log("Touch events enabled for all objects.");
    }

    public void DisableTouchEvents()
    {
        isTouchEventActive = false;
        Debug.Log("Touch events disabled for all objects.");
    }
}
