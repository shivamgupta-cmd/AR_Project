using UnityEngine;

public class PlayAnimationByClip : MonoBehaviour
{
    public Animator animator;

    public void PlayClip(AnimationClip clip)
    {
        if (clip == null) return;

        animator.Play(clip.name, 0, 0f);
    }
}
