using UnityEngine;
using UnityEngine.UI;

public class ButtonToggleSound : MonoBehaviour
{
    public AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip soundClip;  // The sound clip to play

    void Start()
    {
        // Find all Button and Toggle components in the scene
        Button[] buttons = FindObjectsOfType<Button>();
        Toggle[] toggles = FindObjectsOfType<Toggle>();

        // Add listeners for all Buttons
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlaySound);
        }

        // Add listeners for all Toggles
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate { PlaySound(); });
        }
    }

    void PlaySound()
    {
        // Play the sound clip when any button or toggle is tapped/selected
        if (audioSource != null && soundClip != null)
        {
            audioSource.PlayOneShot(soundClip);
        }
    }
}
