using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ImageTargetAudioHandler : MonoBehaviour
{
    [Tooltip("The key of the audio clip to play when this image is detected.")]
    public string audioKey;

    private ObserverBehaviour observerBehaviour;
    private LocalizedAudioPlayer audioManager;

    private void Awake()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        audioManager = FindObjectOfType<LocalizedAudioPlayer>();

        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour target, TargetStatus status)
    {
        if (status.Status == Status.TRACKED)
        {
            Debug.Log($"✅ Image Target '{target.TargetName}' Detected - Playing Audio: {audioKey}");
            audioManager?.PlayAudioClip(audioKey);
        }
        else if (status.Status == Status.NO_POSE || status.Status == Status.LIMITED)
        {
            Debug.Log($"❌ Image Target '{target.TargetName}' Lost - Stopping Audio");
            audioManager?.StopAudio();
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