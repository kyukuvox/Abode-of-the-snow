using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioClip music;
    public float fadeOutDuration = 1f;
    public float fadeInDuration = 1f;
    [Range(0f, 1f)]
    public float volume = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        MusicManager.Instance.PlayMusic(music, fadeOutDuration, fadeInDuration, volume);
    }
}