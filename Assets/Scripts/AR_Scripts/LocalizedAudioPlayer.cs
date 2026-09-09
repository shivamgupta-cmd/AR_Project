using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class LocalizedAudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public class AudioClipData
    {
        public string key;
        public LocalizedAudioClip clip;
    }

    [Header("Audio Clips")]
    public List<AudioClipData> audioClips;

    private Dictionary<string, AudioClip> loadedClips = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Plays an audio clip based on a given key.
    /// </summary>
    public void PlayAudioClip(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("Audio key is empty or null.");
            return;
        }

        if (loadedClips.TryGetValue(key, out var cachedClip))
        {
            audioSource.clip = cachedClip;
            audioSource.Play();
        }
        else
        {
            var clipData = audioClips.Find(c => c.key == key);
            if (clipData != null && clipData.clip != null)
            {
                clipData.clip.LoadAssetAsync().Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var clip = handle.Result;
                        loadedClips[key] = clip;
                        audioSource.clip = clip;
                        audioSource.Play();
                    }
                    else
                    {
                        Debug.LogError($"Failed to load audio clip for key: {key}");
                    }
                };
            }
            else
            {
                Debug.LogWarning($"Audio clip with key '{key}' not found or unassigned.");
            }
        }
    }

    /// <summary>
    /// Stops any currently playing audio.
    /// </summary>
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}