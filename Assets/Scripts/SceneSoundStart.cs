using UnityEngine;

public class SceneSoundStart : MonoBehaviour
{
    public AudioSource audioSource;  // la source du son
    public AudioClip startClip;      // le son à jouer au démarrage

    void Start()
    {
        if (audioSource != null && startClip != null)
        {
            audioSource.clip = startClip;
            audioSource.Play();  // joue le son une fois au démarrage
        }
    }
}
