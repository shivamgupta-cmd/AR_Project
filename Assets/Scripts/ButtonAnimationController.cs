using UnityEngine;

public class ButtonAnimationController : MonoBehaviour
{
    public Animation anim;

    public AnimationClip clip1;
    public AnimationClip clip2;

    void Awake()
    {
        if (!anim)
            anim = GetComponent<Animation>();
    }

    // Button 1
    public void PlayClip1()
    {
        if (!gameObject.activeInHierarchy) return;

        anim.Stop();
        anim.clip = clip1;
        anim.Play();
    }

    // Button 2
    public void PlayClip2()
    {
        if (!gameObject.activeInHierarchy) return;

        anim.Stop();
        anim.clip = clip2;
        anim.Play();
    }
}
