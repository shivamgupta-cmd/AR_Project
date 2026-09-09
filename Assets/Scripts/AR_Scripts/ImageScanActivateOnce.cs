//using UnityEngine;
//using Vuforia;

///// <summary>
///// Uses an ImageTarget only as a one-time trigger. Once detected, the model
///// remains active until the scene is unloaded, even if the target is lost.
///// </summary>
//public sealed class ImageScanActivateOnce : MonoBehaviour
//{
//    [SerializeField] private GameObject modelObject;

//    private ObserverBehaviour observer;
//    private bool hasActivated;
//    private Vector3 initialPosition;
//    private Quaternion initialRotation;
//    private Vector3 initialScale;

//    private void Awake()
//    {
//        observer = GetComponent<ObserverBehaviour>();

//        if (modelObject != null)
//        {
//            initialPosition = modelObject.transform.position;
//            initialRotation = modelObject.transform.rotation;
//            initialScale = modelObject.transform.localScale;
//            modelObject.SetActive(false);
//        }
//    }

//    private void OnEnable()
//    {
//        if (observer == null)
//            observer = GetComponent<ObserverBehaviour>();

//        if (observer != null && !hasActivated)
//            observer.OnTargetStatusChanged += OnTargetStatusChanged;
//    }

//    private void OnTargetStatusChanged(
//        ObserverBehaviour behaviour,
//        TargetStatus status)
//    {
//        if (hasActivated || modelObject == null)
//            return;

//        if (status.Status != Status.TRACKED &&
//            status.Status != Status.EXTENDED_TRACKED)
//            return;

//        hasActivated = true;
//        modelObject.SetActive(true);
//        observer.OnTargetStatusChanged -= OnTargetStatusChanged;

//        Debug.Log("IMAGE SCANNED ONCE - MODEL WILL REMAIN ACTIVE");
//    }

//    public void ResetPosition()
//    {
//        if (!hasActivated || modelObject == null)
//            return;

//        modelObject.transform.SetPositionAndRotation(
//            initialPosition,
//            initialRotation
//        );
//        modelObject.transform.localScale = initialScale;

//        InteractionModeController controller =
//            modelObject.GetComponent<InteractionModeController>();
//        if (controller != null)
//            controller.DisableAll();
//    }

//    private void OnDisable()
//    {
//        if (observer != null)
//            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
//    }
//}


using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

public sealed class ImageScanActivateOnce : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject modelObject;
    [SerializeField] private Camera arCamera;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 1.2f;
    [SerializeField] private Vector3 spawnScale = Vector3.one;
    [SerializeField] private Vector3 spawnRotation = Vector3.zero;

    [Header("Tracking")]
    [SerializeField] private float trackingSettleTime = 0.15f;

    [Header("MODEL ACTIVATED EVENT")]
    [SerializeField] private UnityEvent onModelActivated;

    [Header("MODEL ACTIVATED AUDIO")]
    [SerializeField] private AudioSource modelAudioSource;
    [SerializeField] private AudioClip modelAudioClip;

    private ObserverBehaviour observer;
    private AnchorBehaviour modelAnchor;

    private bool hasActivated = false;
    private bool activationStarted = false;

    private Vector3 resetLocalPosition;
    private Quaternion resetLocalRotation;
    private Vector3 resetLocalScale;
    private Vector3 resetWorldPosition;
    private Quaternion resetWorldRotation;

    private void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();

        if (modelObject != null)
        {
            modelObject.SetActive(false);
        }

        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        if (observer == null)
        {
            observer = GetComponent<ObserverBehaviour>();
        }

        if (observer != null && !hasActivated)
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
            observer.OnTargetStatusChanged += OnTargetStatusChanged;

            // The target may already be tracked when this GameObject is
            // re-enabled after switching from 3D mode to AR mode.
            TryActivateFromStatus(observer.TargetStatus);
        }
    }

    private void OnTargetStatusChanged(
        ObserverBehaviour behaviour,
        TargetStatus status)
    {
        TryActivateFromStatus(status);
    }

    private void TryActivateFromStatus(TargetStatus status)
    {
        if (hasActivated || activationStarted)
            return;

        if (modelObject == null)
            return;

        if (status.Status != Status.TRACKED &&
            status.Status != Status.EXTENDED_TRACKED)
            return;

        activationStarted = true;

        StartCoroutine(ActivateModel());
    }

    private IEnumerator ActivateModel()
    {
        // Vuforia tracking ko thoda settle hone do
        yield return new WaitForSeconds(trackingSettleTime);
        yield return new WaitForEndOfFrame();

        if (modelObject == null)
        {
            activationStarted = false;
            yield break;
        }

        if (arCamera == null)
        {
            arCamera = Camera.main;
        }

        if (arCamera == null &&
            VuforiaBehaviour.Instance != null)
        {
            arCamera =
                VuforiaBehaviour.Instance.GetComponent<Camera>();
        }

        if (arCamera == null)
        {
            Debug.LogError("AR Camera not found.");

            activationStarted = false;
            yield break;
        }

        // =====================================================
        // CAMERA KE SAMNE POSITION
        // =====================================================

        Vector3 spawnPosition =
            arCamera.transform.position +
            arCamera.transform.forward.normalized *
            spawnDistance;

        // Camera ka Y tilt ignore karenge
        // taaki model seedha rahe
        Vector3 flatForward =
            Vector3.ProjectOnPlane(
                arCamera.transform.forward,
                Vector3.up
            );

        if (flatForward.sqrMagnitude < 0.001f)
        {
            flatForward = arCamera.transform.forward;
        }

        Quaternion anchorRotation =
            Quaternion.LookRotation(
                flatForward.normalized,
                Vector3.up
            );

        // =====================================================
        // PURANE PARENT SE DETACH
        // =====================================================

        modelObject.transform.SetParent(null, true);

        // =====================================================
        // VUFORIA ANCHOR CREATE
        // =====================================================

        if (VuforiaBehaviour.Instance == null ||
            VuforiaBehaviour.Instance.ObserverFactory == null)
        {
            Debug.LogError("Vuforia ObserverFactory not found.");

            activationStarted = false;
            yield break;
        }

        modelAnchor =
            VuforiaBehaviour.Instance
            .ObserverFactory
            .CreateAnchorBehaviour(
                "ModelAnchor_" + GetInstanceID(),
                spawnPosition,
                anchorRotation
            );

        if (modelAnchor == null)
        {
            Debug.LogError("Vuforia Anchor creation failed.");

            activationStarted = false;
            yield break;
        }

        // =====================================================
        // MODEL KO ANCHOR PAR LAGAO
        // =====================================================

        modelObject.transform.SetParent(
            modelAnchor.transform,
            false
        );

        modelObject.transform.localPosition =
            Vector3.zero;

        modelObject.transform.localRotation =
            Quaternion.Euler(spawnRotation);

        modelObject.transform.localScale =
            spawnScale;

        // Animator position ko change na kare
        Animator[] animators =
            modelObject.GetComponentsInChildren<Animator>(true);

        foreach (Animator animator in animators)
        {
            animator.applyRootMotion = false;
        }

        // =====================================================
        // RESET VALUES SAVE
        // =====================================================

        resetLocalPosition =
            modelObject.transform.localPosition;

        resetLocalRotation =
            modelObject.transform.localRotation;

        resetLocalScale =
            modelObject.transform.localScale;

        resetWorldPosition =
            modelObject.transform.position;

        resetWorldRotation =
            modelObject.transform.rotation;

        // =====================================================
        // MODEL ACTIVE
        // =====================================================

        modelObject.SetActive(true);

        hasActivated = true;
        activationStarted = false;

        PlayModelAudio();

        // Invoke once, immediately after the scanned model becomes active.
        onModelActivated?.Invoke();

        // Image ko dobara scan karne ki zarurat nahi
        observer.OnTargetStatusChanged -=
            OnTargetStatusChanged;

        Debug.Log(
            "IMAGE SCANNED -> MODEL SPAWNED IN FRONT OF CAMERA -> ANCHORED"
        );
    }

    public void PlayModelAudio()
    {
        if (modelAudioSource == null || modelAudioClip == null)
            return;

        modelAudioSource.Stop();
        modelAudioSource.clip = modelAudioClip;
        modelAudioSource.Play();
    }

    public void ResetPosition()
    {
        if (!hasActivated || modelObject == null)
        {
            Debug.LogWarning("RESET IGNORED: Scan and activate the AR model first.");
            return;
        }

        // Interaction scripts must stop first, otherwise they can overwrite
        // the reset transform again during the same input frame.
        InteractionModeController controller =
            modelObject.GetComponent<InteractionModeController>();

        if (controller == null)
            controller = modelObject.GetComponentInChildren<InteractionModeController>(true);

        if (controller != null)
            controller.DisableAll();

        modelObject.SetActive(true);

        if (modelAnchor != null)
        {
            if (modelObject.transform.parent != modelAnchor.transform)
                modelObject.transform.SetParent(modelAnchor.transform, false);

            modelObject.transform.localPosition = resetLocalPosition;
            modelObject.transform.localRotation = resetLocalRotation;
        }
        else
        {
            // Safe fallback if Vuforia recreated/removed the anchor.
            modelObject.transform.SetPositionAndRotation(
                resetWorldPosition,
                resetWorldRotation
            );
        }

        modelObject.transform.localScale =
            resetLocalScale;

        Rigidbody[] rigidbodies =
            modelObject.GetComponentsInChildren<Rigidbody>(true);

        foreach (Rigidbody body in rigidbodies)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        Debug.Log("MODEL POSITION, ROTATION AND SCALE RESET");
    }

    private void OnDisable()
    {
        if (observer != null)
        {
            observer.OnTargetStatusChanged -=
                OnTargetStatusChanged;
        }
    }

    private void OnDestroy()
    {
        if (observer != null)
        {
            observer.OnTargetStatusChanged -=
                OnTargetStatusChanged;
        }

        if (modelAnchor != null)
        {
            modelAnchor.UnconfigureAnchor();
        }
    }
}
