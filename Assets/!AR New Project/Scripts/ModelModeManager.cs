using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Vuforia;

public class ModelModeManager : MonoBehaviour
{

    public static ModelModeManager instance;

    [Header("CAMERA BACKGROUND")]
    [SerializeField] private Material modelSkybox;

    [Header("X-RAY")]
    [SerializeField] private XRayModeController xRayController;

    [Header("AR CONTROL")]
    [SerializeField] private ARModeButtonController arButtonController;

    [Header("Text Mode")]
    [SerializeField] private TMP_Text m_modeText;


    [Header("BUTTONS")]
    [SerializeField] private Button mode3DButton;
    [SerializeField] private Button xRayModeButton;
    [SerializeField] private Button arModeButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button quizButton;


    [SerializeField] private Button tutorialPanelFinishButton;

    [SerializeField] private Button infoCancleButton;
    [SerializeField] private Button quizCancleButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button header3DButton;


    [Header("UNITY EVENTS")]
    [SerializeField] private UnityEvent on3DMode;
    [SerializeField] private UnityEvent onXRayMode;
    [SerializeField] private UnityEvent onARMode;
    [SerializeField] private UnityEvent onInfoMode;
    [SerializeField] private UnityEvent onQuizMode;
    [SerializeField] private UnityEvent onInfoCancel;
    [SerializeField] private UnityEvent onQuizCancel;


    [Header("PANELS")]
    [SerializeField] private GameObject model3DPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject arModePanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject quizPanel;

    [Header("QUIZ CONTROL")]
    [SerializeField] private QuizManager quizManager;

    [Header("INFO VOICE")]
    [SerializeField] private AudioSource infoVoicePlayer;
    [SerializeField] private AudioClip infoVoiceClip;


    [Header("ANIMATION SLIDER")]
    [SerializeField] private GameObject animationSlider;


    [Header("3D MODEL")]
    [SerializeField] private GameObject model3DObject;
    [SerializeField] private GameObject modelArObject;


    [Header("VUFORIA AR")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private VuforiaBehaviour vuforiaBehaviour;
    [SerializeField] private GameObject imageTarget;
    [SerializeField] private GameObject[] multipleimageTarget;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        CreateHeader3DButtonIfNeeded();

        // =========================
        // BUTTON LISTENERS
        // =========================

        if (mode3DButton != null)
            mode3DButton.onClick.AddListener(Open3DMode);

        if (xRayModeButton != null)
            xRayModeButton.onClick.AddListener(OpenXRayMode);

        if (arModeButton != null)
            arModeButton.onClick.AddListener(OpenARMode);

        if (infoButton != null)
            infoButton.onClick.AddListener(OpenInfo);

        if (quizButton != null)
            quizButton.onClick.AddListener(OpenQuiz);

        if (infoCancleButton != null)
            infoCancleButton.onClick.AddListener(CloseInfo);

        if (quizCancleButton != null)
            quizCancleButton.onClick.AddListener(CloseQuiz);

        if (header3DButton != null)
            header3DButton.onClick.AddListener(ReloadCurrentScene);

        if (tutorialPanelFinishButton != null)
            tutorialPanelFinishButton.onClick.AddListener(ActiveSetVuforiaMode);


        // =========================
        // AR CAMERA
        // =========================

        if (arCamera != null)
        {
            arCamera.gameObject.SetActive(true);
        }


        // =========================
        // DEFAULT MODE
        // =========================

        Open3DMode();
    }


    // =========================================================
    // 3D MODE
    // =========================================================

    public void Open3DMode()
    {
        Debug.Log("3D MODE");

        StopInfoAndQuizVoice();

        if (m_modeText != null)
            m_modeText.text = "3D object mode";


        if (model3DPanel != null)
            model3DPanel.SetActive(true);

        if (model3DObject != null)
            model3DObject.SetActive(true);

        if (modelArObject != null)
            modelArObject.SetActive(false);

        if (arModePanel != null)
            arModePanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(false);


        if (arButtonController != null)
            arButtonController.DisableARMode();


        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);


        SetVuforiaMode(false);


        if (arCamera != null)
        {
            arCamera.clearFlags = CameraClearFlags.Skybox;

            if (modelSkybox != null)
                RenderSettings.skybox = modelSkybox;
        }


        if (xRayController != null)
            xRayController.DisableXRay();


        if (animationSlider != null)
            animationSlider.SetActive(true);


        if (mode3DButton != null)
            mode3DButton.gameObject.SetActive(false);

        if (xRayModeButton != null)
            xRayModeButton.gameObject.SetActive(true);

        if (backButton != null)
            backButton.gameObject.SetActive(true);

        if (header3DButton != null)
            header3DButton.gameObject.SetActive(false);


        // UnityEvent
        if (on3DMode != null)
            on3DMode.Invoke();
    }


    // =========================================================
    // CLOSE INFO
    // =========================================================

    public void CloseInfo()
    {
        Debug.Log("INFO CLOSED");

        StopInfoAndQuizVoice();

        Open3DMode();


        if (onInfoCancel != null)
            onInfoCancel.Invoke();
    }


    // =========================================================
    // CLOSE QUIZ
    // =========================================================

    public void CloseQuiz()
    {
        Debug.Log("QUIZ CLOSED");

        StopInfoAndQuizVoice();

        Open3DMode();


        if (onQuizCancel != null)
            onQuizCancel.Invoke();
    }


    // =========================================================
    // X-RAY MODE
    // =========================================================

    public void OpenXRayMode()
    {
        Debug.Log("X-RAY MODE");

        StopInfoAndQuizVoice();

        if (m_modeText != null)
            m_modeText.text = "X-Ray mode";


        if (model3DPanel != null)
            model3DPanel.SetActive(true);

        if (model3DObject != null)
            model3DObject.SetActive(true);

        if (arModePanel != null)
            arModePanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(false);


        if (arButtonController != null)
            arButtonController.DisableARMode();


        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);


        SetVuforiaMode(false);


        if (arCamera != null)
        {
            arCamera.clearFlags = CameraClearFlags.SolidColor;
            arCamera.backgroundColor = Color.black;
        }


        if (xRayController != null)
            xRayController.EnableXRay();


        if (animationSlider != null)
            animationSlider.SetActive(true);


        if (xRayModeButton != null)
            xRayModeButton.gameObject.SetActive(false);

        if (mode3DButton != null)
            mode3DButton.gameObject.SetActive(true);

        if (backButton != null)
            backButton.gameObject.SetActive(true);

        if (header3DButton != null)
            header3DButton.gameObject.SetActive(false);


        // UnityEvent
        if (onXRayMode != null)
            onXRayMode.Invoke();
    }


    // =========================================================
    // AR MODE
    // =========================================================

    public void ActiveSetVuforiaMode()
    {
        SetVuforiaMode(true);

    }


    public void OpenARMode()
    {
        Debug.Log("AR MODE");

        StopInfoAndQuizVoice();

        // Always start Vuforia when entering AR mode. The tutorial Finish
        // button may call this again, which is safe and keeps older scenes working.
        SetVuforiaMode(true);

        if (m_modeText != null)
            m_modeText.text = "AR mode";


        if (model3DPanel != null)
            model3DPanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (arModePanel != null)
            arModePanel.SetActive(true);


        if (model3DObject != null)
            model3DObject.SetActive(false);


        if (animationSlider != null)
            animationSlider.SetActive(false);


        if (arCamera != null)
        {
            arCamera.clearFlags = CameraClearFlags.SolidColor;
            arCamera.backgroundColor = Color.black;
        }


        if (xRayController != null)
            xRayController.DisableXRay();


        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);




        if (arButtonController != null)
            arButtonController.EnableARMode();


        // Header 3D button returns from AR without changing the Down Panel buttons.
        if (header3DButton != null)
            header3DButton.gameObject.SetActive(true);

        if (backButton != null)
            backButton.gameObject.SetActive(false);


        // UnityEvent
        if (onARMode != null)
            onARMode.Invoke();
    }


    // =========================================================
    // INFO MODE
    // =========================================================

    public void OpenInfo()
    {
        Debug.Log("INFO MODE");

        StopInfoAndQuizVoice();

        if (m_modeText != null)
            m_modeText.text = "Info mode";


        if (model3DPanel != null)
            model3DPanel.SetActive(false);

        if (arModePanel != null)
            arModePanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(true);

        if (infoVoicePlayer != null && infoVoiceClip != null)
        {
            infoVoicePlayer.clip = infoVoiceClip;
            infoVoicePlayer.Play();
        }


        if (animationSlider != null)
            animationSlider.SetActive(false);


        if (arButtonController != null)
            arButtonController.DisableARMode();


        SetVuforiaMode(false);


        if (xRayController != null)
            xRayController.DisableXRay();


        Reset3DXRayButtons();

        if (backButton != null)
            backButton.gameObject.SetActive(false);

        if (header3DButton != null)
            header3DButton.gameObject.SetActive(false);


        // UnityEvent
        if (onInfoMode != null)
            onInfoMode.Invoke();
    }


    // =========================================================
    // QUIZ MODE
    // =========================================================

    public void OpenQuiz()
    {
        Debug.Log("QUIZ MODE");

        StopInfoAndQuizVoice();

        if (m_modeText != null)
            m_modeText.text = "Quiz mode";


        if (model3DPanel != null)
            model3DPanel.SetActive(false);

        if (arModePanel != null)
            arModePanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (quizManager != null)
            quizManager.StartQuiz();


        if (animationSlider != null)
            animationSlider.SetActive(false);


        if (arButtonController != null)
            arButtonController.DisableARMode();


        SetVuforiaMode(false);


        if (xRayController != null)
            xRayController.DisableXRay();


        Reset3DXRayButtons();

        if (backButton != null)
            backButton.gameObject.SetActive(false);

        if (header3DButton != null)
            header3DButton.gameObject.SetActive(false);


        // UnityEvent
        if (onQuizMode != null)
            onQuizMode.Invoke();
    }


    // =========================================================
    // VUFORIA
    // =========================================================

    private void SetVuforiaMode(bool active)
    {
        if (arCamera != null)
            arCamera.gameObject.SetActive(true);


        if (vuforiaBehaviour != null)
            vuforiaBehaviour.enabled = active;


        if (imageTarget != null)
            imageTarget.SetActive(active);

        if (multipleimageTarget != null)
        {
            foreach (GameObject target in multipleimageTarget)
            {
                if (target != null)
                    target.SetActive(active);
            }
        }



        Debug.Log("Vuforia Mode = " + active);
    }


    // =========================================================
    // RESET 3D / X-RAY BUTTONS
    // =========================================================

    private void Reset3DXRayButtons()
    {
        if (mode3DButton != null)
            mode3DButton.gameObject.SetActive(false);

        if (xRayModeButton != null)
            xRayModeButton.gameObject.SetActive(true);
    }

    private void StopInfoAndQuizVoice()
    {
        if (infoVoicePlayer != null)
        {
            infoVoicePlayer.Stop();
            infoVoicePlayer.clip = null;
        }

        if (quizManager != null)
            quizManager.StopQuizAudio();
    }

    private void CreateHeader3DButtonIfNeeded()
    {
        if (header3DButton != null || mode3DButton == null || backButton == null)
            return;

        header3DButton = Instantiate(mode3DButton, backButton.transform.parent);
        header3DButton.gameObject.name = "Header 3D Mode Button";
        header3DButton.onClick.RemoveAllListeners();

        RectTransform headerRect = header3DButton.transform as RectTransform;
        RectTransform backRect = backButton.transform as RectTransform;

        if (headerRect != null && backRect != null)
        {
            headerRect.anchorMin = backRect.anchorMin;
            headerRect.anchorMax = backRect.anchorMax;
            headerRect.pivot = backRect.pivot;
            headerRect.anchoredPosition = backRect.anchoredPosition;
            headerRect.sizeDelta = backRect.sizeDelta;
            headerRect.localRotation = backRect.localRotation;
            headerRect.localScale = backRect.localScale;
            headerRect.SetSiblingIndex(backRect.GetSiblingIndex() + 1);
        }

        if (header3DButton.GetComponent<ButtonHoverFeedback>() == null)
            header3DButton.gameObject.AddComponent<ButtonHoverFeedback>();

        header3DButton.gameObject.SetActive(false);
    }

    private void ReloadCurrentScene()
    {
        StopInfoAndQuizVoice();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }


    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        if (mode3DButton != null)
            mode3DButton.onClick.RemoveListener(Open3DMode);

        if (xRayModeButton != null)
            xRayModeButton.onClick.RemoveListener(OpenXRayMode);

        if (arModeButton != null)
            arModeButton.onClick.RemoveListener(OpenARMode);

        if (infoButton != null)
            infoButton.onClick.RemoveListener(OpenInfo);

        if (quizButton != null)
            quizButton.onClick.RemoveListener(OpenQuiz);

        if (infoCancleButton != null)
            infoCancleButton.onClick.RemoveListener(CloseInfo);

        if (quizCancleButton != null)
            quizCancleButton.onClick.RemoveListener(CloseQuiz);

        if (header3DButton != null)
            header3DButton.onClick.RemoveListener(ReloadCurrentScene);
    }
}
