using UnityEngine;

public class PartAudioPlayer : MonoBehaviour
{
    public AudioClip partAudio;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
    }

    //void Update()
    //{
    //    // 🔒 Agar disassemble mode ON nahi hai to kuch mat karo
    //    if (!DisassembleManager.isDisassembled)
    //        return;

    //    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            if (hit.transform == transform)
    //            {
    //                if (partAudio != null && audioSource != null)
    //                {
    //                    audioSource.Stop();
    //                    audioSource.PlayOneShot(partAudio);
    //                }
    //            }
    //        }
    //    }
    //}

    public void PartsVoPlay()
    {
        if (!DisassembleManager.isDisassembled)
            return;
        if (partAudio != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(partAudio);
        }
    }
}