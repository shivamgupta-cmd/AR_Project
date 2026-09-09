using UnityEngine;
using System.Collections;

public class SingleAnimationAudioController : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator; // The Animator controlling your FBX
    public AnimationClip animationClip; // Your single animation clip

    [Header("Audio Settings")]
    public AudioClip[] audioClips; // Audio clips to play at pause points
    public float[] pauseTimesNormalized; // When to pause (0-1 values, e.g., 0.5 = halfway)
    public float transitionDelay = 0.5f; // Delay before restarting animation

    private AudioSource audioSource;
    private bool isPlaying = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (animator == null)
            animator = GetComponent<Animator>();

        StartAnimationSequence();
    }

    public void StartAnimationSequence()
    {
        if (!isPlaying)
            StartCoroutine(PlayAnimationWithAudioPauses());
    }

    IEnumerator PlayAnimationWithAudioPauses()
    {
        isPlaying = true;

        while (true) // Loop forever
        {
            animator.Play(animationClip.name);

            for (int i = 0; i < pauseTimesNormalized.Length; i++)
            {
                // Wait until the pause point
                float pauseTime = animationClip.length * pauseTimesNormalized[i];
                yield return new WaitForSeconds(pauseTime);

                // Pause the animation
                animator.speed = 0;

                // Play the corresponding audio clip
                if (i < audioClips.Length && audioClips[i] != null)
                {
                    audioSource.clip = audioClips[i];
                    audioSource.Play();
                    yield return new WaitForSeconds(audioClips[i].length);
                }

                // Resume animation
                animator.speed = 1;
            }

            // Wait for the remaining animation time
            yield return new WaitForSeconds(animationClip.length * (1 - pauseTimesNormalized[^1]));

            // Optional delay before restarting
            yield return new WaitForSeconds(transitionDelay);
        }
    }

    // Call this via Animation Event if using events instead of time-based pauses
    public void PauseAndPlayAudio(int audioIndex)
    {
        if (audioIndex >= 0 && audioIndex < audioClips.Length)
        {
            StartCoroutine(PauseForAudio(audioIndex));
        }
    }

    IEnumerator PauseForAudio(int audioIndex)
    {
        animator.speed = 0;
        audioSource.clip = audioClips[audioIndex];
        audioSource.Play();
        yield return new WaitForSeconds(audioClips[audioIndex].length);
        animator.speed = 1;
    }

    public void StopAnimationSequence()
    {
        StopAllCoroutines();
        isPlaying = false;
        animator.speed = 1;
    }
}