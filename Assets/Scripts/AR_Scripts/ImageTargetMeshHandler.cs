using UnityEngine;
using Vuforia;

public class ImageTargetMeshHandler : MonoBehaviour
{
    private ObserverBehaviour observerBehaviour;

    private void Awake()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        // Initially disable all children
        SetChildrenActive(false);
    }

    private void OnTargetStatusChanged(ObserverBehaviour target, TargetStatus status)
    {
        if (status.Status == Status.TRACKED)
        {
            Debug.Log($"✅ Image Target '{target.TargetName}' Detected - Enabling Children");
            SetChildrenActive(true);
        }
        else if (status.Status == Status.NO_POSE || status.Status == Status.LIMITED)
        {
            Debug.Log($"❌ Image Target '{target.TargetName}' Lost - Disabling Children");
            SetChildrenActive(false);
        }
    }

    /// <summary>
    /// Enables or disables all child GameObjects under this Image Target.
    /// </summary>
    private void SetChildrenActive(bool state)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(state);  // Enable/Disable entire GameObject
        }
    }

    private void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}
