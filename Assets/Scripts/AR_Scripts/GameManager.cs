using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Animator animator;
    public Button assembleButton;
    public Button dissembleButton;
    public GameObject[] ToggleGameobjects;
    public Collider[] TouchCollider;
    public GameObject[] labels;
    public LeanSelectableByFinger[] leanSelectableByFingers; // Array of LeanSelectableByFinger components

    private bool isDissembling = false;
    private bool isDissembled = false; // Tracks if the object is dissembled

    void Start()
    {
        assembleButton.onClick.AddListener(OnAssembleButtonClicked);
        dissembleButton.onClick.AddListener(OnDissembleButtonClicked);
        ToggleGameobjects[0].SetActive(false); // Dissemble GameObject is initially inactive
        ToggleGameobjects[1].SetActive(true);  // Assemble GameObject is initially active

        UpdateColliderAndLabelState();
    }

    void OnAssembleButtonClicked()
    {
        if (!isDissembling)
        {
            animator.Play("Assemble");

            LabelController.instance.HideAllLabels();

            // Toggle GameObjects
            ToggleGameobjects[1].SetActive(true);  // Enable Assemble GameObject
            ToggleGameobjects[0].SetActive(false); // Disable Dissemble GameObject

            isDissembled = false; // Reset dissembled state when assembling
            UpdateColliderAndLabelState();
        }
    }

    void OnDissembleButtonClicked()
    {
        if (!isDissembling)
        {
            isDissembling = true;
            animator.Play("Dissemble");


            LabelController.instance.ShowAllLabels();


            // Toggle GameObjects
            ToggleGameobjects[0].SetActive(true);  // Enable Dissemble GameObject
            ToggleGameobjects[1].SetActive(false); // Disable Assemble GameObject

            // Wait for the dissemble animation to complete
            StartCoroutine(WaitForDissembleAnimation());

            // Update collider state immediately to disable
            UpdateColliderAndLabelState();
        }
    }

    IEnumerator WaitForDissembleAnimation()
    {
        float dissembleDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dissembleDuration);
        isDissembling = false;
        isDissembled = true; // Set dissembled state to true after animation
        UpdateColliderAndLabelState();
    }

    private void UpdateColliderAndLabelState()
    {
        bool isColliderEnabled = isDissembled; // Colliders are enabled only when dissembled

        // Enable/disable colliders
        foreach (var collider in TouchCollider)
        {
            if (collider != null)
            {
                collider.enabled = isColliderEnabled;
            }
        }

        // Enable/disable LeanSelectableByFinger components
        foreach (var leanSelectable in leanSelectableByFingers)
        {
            if (leanSelectable != null)
            {
                leanSelectable.enabled = isColliderEnabled;
            }
        }

        // Enable/disable labels
        if (isDissembled)
        {
            LabelOn();
        }
        else
        {
            LabelOff();
        }
    }

    public void LabelOn()
    {
        foreach (var label in labels)
        {
            if (label != null)
            {
                label.gameObject.SetActive(true);
            }
        }
    }

    public void LabelOff()
    {
        foreach (var label in labels)
        {
            if (label != null)
            {
                label.gameObject.SetActive(false);
            }
        }
    }
}