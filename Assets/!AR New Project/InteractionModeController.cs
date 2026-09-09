using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lean.Touch;
using UnityEngine.Events;

public class InteractionModeController : MonoBehaviour
{
    [Header("Interaction Scripts")]
    public LeanDragTranslate drag;
    public LeanPinchScale pinch;
    public OneFingerFixedRotate rotateScript;

    public ImageScanActivateOnce imageScanActivateOnce;
    public ImageScanActivateOnce[] multipleImageScanActivateOnce;


    [Header("Buttons")]
    public Button resetButton;
    public Button moveButton;
    public Button rotateButton;
    public Button scaleButton;


    [Header("Button Audio")]
    public AudioClip m_audioClipBtnClick;
    public AudioSource m_audioSourceBtnClick;


    [Header("Active Script")]
    public UnityEvent IsActive;


    public bool IsMoveEnabled =>
        drag != null && drag.enabled;

    public bool IsRotateEnabled =>
        rotateScript != null && rotateScript.enabled;

    public bool IsScaleEnabled =>
        pinch != null && pinch.enabled;


    private void Start()
    {
        DisableAll();

        if (moveButton != null)
            moveButton.onClick.AddListener(EnableMove);

        if (scaleButton != null)
            scaleButton.onClick.AddListener(EnableScale);

        if (rotateButton != null)
            rotateButton.onClick.AddListener(EnableRotate);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetModel);

        IsActive?.Invoke();
    }


    // =====================================================
    // DISABLE ALL
    // =====================================================

    public void DisableAll()
    {
        if (drag != null)
            drag.enabled = false;

        if (pinch != null)
            pinch.enabled = false;

        if (rotateScript != null)
            rotateScript.enabled = false;
    }


    // =====================================================
    // MOVE
    // =====================================================

    public void EnableMove()
    {
        DisableAll();

        if (drag != null)
            drag.enabled = true;

        SelectButton(moveButton);

        PlayBtnClickClip();

        Debug.Log("MOVE MODE");
    }


    // =====================================================
    // SCALE
    // =====================================================

    public void EnableScale()
    {
        DisableAll();

        if (pinch != null)
            pinch.enabled = true;

        SelectButton(scaleButton);

        PlayBtnClickClip();

        Debug.Log("SCALE MODE");
    }


    // =====================================================
    // ROTATE
    // =====================================================

    public void EnableRotate()
    {
        DisableAll();

        if (rotateScript != null)
            rotateScript.enabled = true;

        SelectButton(rotateButton);

        PlayBtnClickClip();

        Debug.Log("ROTATE MODE");
    }


    // =====================================================
    // RESET
    // =====================================================

    public void ResetModel()
    {
        DisableAll();
        if (imageScanActivateOnce != null)
            imageScanActivateOnce.ResetPosition();

        if (multipleImageScanActivateOnce != null)
        {
            foreach (ImageScanActivateOnce imageScan in multipleImageScanActivateOnce)
            {
                if (imageScan != null)
                {
                    imageScan.ResetPosition();
                }
            }
        }


        SelectButton(resetButton);

        PlayBtnClickClip();

        Debug.Log("MODEL RESET");
    }


    // =====================================================
    // SELECT BUTTON
    // =====================================================

    private void SelectButton(Button button)
    {
        if (button == null)
            return;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                button.gameObject
            );
        }

        button.Select();
    }


    // =====================================================
    // AUDIO
    // =====================================================

    public void PlayBtnClickClip()
    {
        if (m_audioSourceBtnClick == null ||
            m_audioClipBtnClick == null)
            return;

        m_audioSourceBtnClick.PlayOneShot(
            m_audioClipBtnClick
        );
    }


    private void OnDestroy()
    {
        if (moveButton != null)
            moveButton.onClick.RemoveListener(EnableMove);

        if (scaleButton != null)
            scaleButton.onClick.RemoveListener(EnableScale);

        if (rotateButton != null)
            rotateButton.onClick.RemoveListener(EnableRotate);

        if (resetButton != null)
            resetButton.onClick.RemoveListener(ResetModel);
    }
}