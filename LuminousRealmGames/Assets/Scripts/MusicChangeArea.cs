using UnityEngine;

public class MusicChangeArea : MonoBehaviour
{
    public AudioClip area2Music;
    public AudioSource musicSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            musicSource.Stop();
            musicSource.clip = area2Music;
            musicSource.Play();
        }
    }
}
