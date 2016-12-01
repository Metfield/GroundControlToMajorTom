using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_clip1;

    [SerializeField]
    private AudioClip m_clip2;

    [SerializeField]
    private AudioSource m_audioSource;
    
    public void PlayOneShot(AudioClip clip)
    {
        m_audioSource.PlayOneShot(clip);
    }
}
