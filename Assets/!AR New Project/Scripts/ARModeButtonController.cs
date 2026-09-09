using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ARModeButtonController : MonoBehaviour
{
    [Header("VUFORIA IMAGE TARGET")]
    [SerializeField] private ObserverBehaviour imageTargetObserver;

    [Header("MULTIPLE IMAGE TARGETS")]
    [SerializeField] private ObserverBehaviour[] multipleImageTargetObserver;

    [Header("AR CONTROL BUTTONS")]
    [SerializeField] private Button[] arButtons;

    private bool isARModeActive;

    // Currently tracked targets
    private readonly HashSet<ObserverBehaviour> trackedTargets =
        new HashSet<ObserverBehaviour>();


    private void Start()
    {
        SetButtonsInteractable(false);

        SubscribeTargets();
    }


    // =========================================================
    // SUBSCRIBE TARGETS
    // =========================================================

    private void SubscribeTargets()
    {
        // Main Image Target
        if (imageTargetObserver != null)
        {
            imageTargetObserver.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        // Additional Image Targets
        if (multipleImageTargetObserver != null)
        {
            foreach (ObserverBehaviour observer in multipleImageTargetObserver)
            {
                if (observer != null)
                {
                    observer.OnTargetStatusChanged += OnTargetStatusChanged;
                }
            }
        }
    }


    // =========================================================
    // ENABLE AR MODE
    // =========================================================

    public void EnableARMode()
    {
        Debug.Log("AR Control Mode ON");

        isARModeActive = true;

        trackedTargets.Clear();

        CheckCurrentTargets();
    }


    // =========================================================
    // DISABLE AR MODE
    // =========================================================

    public void DisableARMode()
    {
        Debug.Log("AR Control Mode OFF");

        isARModeActive = false;

        trackedTargets.Clear();

        SetButtonsInteractable(false);
    }


    // =========================================================
    // TARGET STATUS CHANGED
    // =========================================================

    private void OnTargetStatusChanged(
        ObserverBehaviour behaviour,
        TargetStatus targetStatus)
    {
        if (!isARModeActive)
        {
            SetButtonsInteractable(false);
            return;
        }


        bool targetFound = IsTargetFound(targetStatus);


        if (targetFound)
        {
            trackedTargets.Add(behaviour);

            Debug.Log(
                "Image Target Found : " +
                behaviour.TargetName
            );
        }
        else
        {
            trackedTargets.Remove(behaviour);

            Debug.Log(
                "Image Target Lost : " +
                behaviour.TargetName
            );
        }


        // Buttons stay active if AT LEAST ONE target is tracked
        SetButtonsInteractable(trackedTargets.Count > 0);
    }


    // =========================================================
    // CHECK CURRENT TARGETS
    // =========================================================

    private void CheckCurrentTargets()
    {
        trackedTargets.Clear();


        // Main Target
        CheckObserver(imageTargetObserver);


        // Multiple Targets
        if (multipleImageTargetObserver != null)
        {
            foreach (ObserverBehaviour observer in multipleImageTargetObserver)
            {
                CheckObserver(observer);
            }
        }


        SetButtonsInteractable(trackedTargets.Count > 0);
    }


    private void CheckObserver(ObserverBehaviour observer)
    {
        if (observer == null)
            return;


        TargetStatus status = observer.TargetStatus;


        if (IsTargetFound(status))
        {
            trackedTargets.Add(observer);
        }
    }


    // =========================================================
    // CHECK FOUND STATUS
    // =========================================================

    private bool IsTargetFound(TargetStatus targetStatus)
    {
        return
            targetStatus.Status == Status.TRACKED ||
            targetStatus.Status == Status.EXTENDED_TRACKED;
    }


    // =========================================================
    // BUTTON CONTROL
    // =========================================================

    private void SetButtonsInteractable(bool value)
    {
        if (arButtons == null)
            return;


        foreach (Button button in arButtons)
        {
            if (button != null)
            {
                button.interactable = value;
            }
        }
    }


    // =========================================================
    // UNSUBSCRIBE
    // =========================================================

    private void OnDestroy()
    {
        // Main Target
        if (imageTargetObserver != null)
        {
            imageTargetObserver.OnTargetStatusChanged -= OnTargetStatusChanged;
        }


        // Multiple Targets
        if (multipleImageTargetObserver != null)
        {
            foreach (ObserverBehaviour observer in multipleImageTargetObserver)
            {
                if (observer != null)
                {
                    observer.OnTargetStatusChanged -= OnTargetStatusChanged;
                }
            }
        }
    }
}