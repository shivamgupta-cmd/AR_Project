using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSettingsSO audioSettings;
    public List<AudioSource> sourcesToMute; // Drop specific AudioSources here

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ApplyMuteState();

        // Subscribe to sceneLoaded to reapply mute after scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ToggleMute()
    {
        audioSettings.isMuted = !audioSettings.isMuted;
        ApplyMuteState();
    }

    public void ApplyMuteState()
    {
        foreach (AudioSource source in sourcesToMute)
        {
            if (source != null)
                source.mute = audioSettings.isMuted;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reapply mute to existing list after a new scene is loaded
        ApplyMuteState();
    }

    // Optional: call this if you want to set sources dynamically
    public void RefreshSources(List<AudioSource> newSources)
    {
        sourcesToMute = newSources;
        ApplyMuteState();
    }
}
