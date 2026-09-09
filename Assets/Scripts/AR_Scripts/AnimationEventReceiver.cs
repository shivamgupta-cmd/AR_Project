using UnityEngine;
using UnityEngine.Events;


public class AnimationEventReceiver : MonoBehaviour


{

    [SerializeField]
    public AudioClip MetalSound;
    private AudioSource Metalaudio;
    [SerializeField]
    public UnityEvent Animationtrigger;

    public void Start()
    {
        Metalaudio = gameObject.AddComponent<AudioSource>();
        Metalaudio.clip = MetalSound;
        Metalaudio.playOnAwake = false;
    }
    // Called from animation event at a specific frame
    public void PlaySwingSound()
    {
        Debug.Log("Swing sound triggered!");
        Metalaudio.Play();
        Animationtrigger.Invoke();  
      
    }

   
}
