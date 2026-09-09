using System.Collections;
using UnityEngine;
using Vuforia;

public class ImageOnceAppear : MonoBehaviour
{
    [Header("References")]
    public GameObject modelObject;
    public Camera arCamera;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clip;

    [Header("Spawn Settings")]
    [Min(0.1f)]
    public float spawnDistance = 1.2f;

    public Vector3 spawnScale = Vector3.one;
    public Vector3 spawnRotation = Vector3.zero;

    private ObserverBehaviour observer;
    private AnchorBehaviour modelAnchor;
    private bool isImageTargetFallback;
    private Vector3 fallbackSpawnPosition;
    private Quaternion fallbackSpawnRotation;
    private InteractionModeController interactionController;

    private bool isSpawned = false;
    private bool isSingleTime = false;

    public static ImageOnceAppear Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        observer = GetComponent<ObserverBehaviour>();

        if (observer == null)
        {
            Debug.LogError("ObserverBehaviour NOT FOUND");
            return;
        }

        observer.OnTargetStatusChanged += OnStatusChanged;

        if (modelObject != null)
        {
            modelObject.SetActive(false);
        }

        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    // =====================================================
    // IMAGE TARGET
    // =====================================================

    private void OnStatusChanged(
        ObserverBehaviour behaviour,
        TargetStatus status)
    {
        if (isSpawned)
            return;

        if (status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED)
        {
            StartCoroutine(SpawnModel());
        }
    }

    // =====================================================
    // SPAWN MODEL
    // =====================================================

    private IEnumerator SpawnModel()
    {
        if (isSpawned)
            yield break;

        if (modelObject == null)
            yield break;

        if (arCamera == null)
            arCamera = Camera.main;

        if (arCamera == null &&
            VuforiaBehaviour.Instance != null)
        {
            arCamera =
                VuforiaBehaviour.Instance.GetComponent<Camera>();
        }

        if (arCamera == null)
        {
            Debug.LogError("AR Camera not found");
            yield break;
        }

        isSpawned = true;

        yield return new WaitForSeconds(0.2f);

        Vector3 spawnPosition =
            arCamera.transform.position +
            arCamera.transform.forward.normalized *
            spawnDistance;

        Vector3 flatForward =
            Vector3.ProjectOnPlane(
                arCamera.transform.forward,
                Vector3.up
            );

        if (flatForward.sqrMagnitude < 0.0001f)
            flatForward = Vector3.forward;

        Quaternion spawnWorldRotation =
            Quaternion.LookRotation(
                flatForward.normalized,
                Vector3.up
            );

        // ===============================================
        // CREATE VUFORIA ANCHOR
        // ===============================================

        if (VuforiaBehaviour.Instance != null &&
            VuforiaBehaviour.Instance.ObserverFactory != null)
        {
            try
            {
                modelAnchor =
                    VuforiaBehaviour.Instance
                    .ObserverFactory
                    .CreateAnchorBehaviour(
                        "ModelAnchor_" + GetInstanceID(),
                        spawnPosition,
                        spawnWorldRotation
                    );
            }
            catch (System.Exception exception)
            {
                modelAnchor = null;
                Debug.LogWarning(
                    "Vuforia Mid-Air Anchor unavailable; using ImageTarget fallback. " +
                    exception.Message
                );
            }
        }

        // ===============================================
        // ANCHOR SUCCESS
        // ===============================================

        if (modelAnchor != null)
        {
            isImageTargetFallback = false;
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
        }

        else
        {
            // Some Vuforia-supported image-tracking devices don't support
            // mid-air anchors. Keep the model active and target-tracked rather
            // than leaving a blank AR screen or attaching it to the camera.
            modelObject.transform.SetParent(null, true);
            modelObject.transform.SetPositionAndRotation(
                spawnPosition,
                spawnWorldRotation * Quaternion.Euler(spawnRotation)
            );
            modelObject.transform.localScale = spawnScale;
            fallbackSpawnPosition = modelObject.transform.position;
            fallbackSpawnRotation = modelObject.transform.rotation;
            isImageTargetFallback = true;

            Debug.LogWarning(
                "MODEL ACTIVE WITH VUFORIA IMAGE TARGET FALLBACK"
            );
        }

        Animator[] animators =
            modelObject.GetComponentsInChildren<Animator>(true);

        foreach (Animator animator in animators)
        {
            animator.applyRootMotion = false;
        }

        modelObject.SetActive(true);

        ActiveObjClip();

        observer.OnTargetStatusChanged -= OnStatusChanged;

        Debug.Log("MODEL ACTIVE AND READY");
    }

    // =====================================================
    // AUDIO
    // =====================================================

    public void ActiveObjClip()
    {
        if (isSingleTime)
            return;

        if (audioSource == null ||
            clip == null)
            return;

        audioSource.clip = clip;
        audioSource.Play();

        isSingleTime = true;
    }

    // =====================================================
    // RESET
    // =====================================================

    public void ResetPosition()
    {
        if (modelObject == null)
            return;

        if (modelAnchor != null)
        {
            modelObject.transform.localPosition = Vector3.zero;
            modelObject.transform.localRotation =
                Quaternion.Euler(spawnRotation);
        }
        else if (isImageTargetFallback)
        {
            modelObject.transform.SetPositionAndRotation(
                fallbackSpawnPosition,
                fallbackSpawnRotation
            );
        }
        else
        {
            return;
        }

        modelObject.transform.localScale =
            spawnScale;

        if (interactionController == null)
            interactionController = modelObject.GetComponent<InteractionModeController>();

        if (interactionController != null)
            interactionController.DisableAll();

        Debug.Log("MODEL POSITION RESET");
    }

    private void OnDestroy()
    {
        if (observer != null)
        {
            observer.OnTargetStatusChanged -=
                OnStatusChanged;
        }

        if (modelAnchor != null)
        {
            modelAnchor.UnconfigureAnchor();
        }
    }
}
