using UnityEngine;
using UnityEngine.UI;
 using TMPro;
public class MuteButton : MonoBehaviour
{
    public AudioManager audioManager;
    public TextMeshProUGUI buttonText;
 
    void Start()
    {
        UpdateButtonUI();
    }
 
    public void OnClickToggleMute()
    {
        audioManager.ToggleMute();
        UpdateButtonUI();
    }
 
    void UpdateButtonUI()
    {
        buttonText.text = audioManager.audioSettings.isMuted ? "Unmute" : "Mute";
    }
}