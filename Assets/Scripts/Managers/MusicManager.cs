using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Sources audio")]
    public AudioSource audioSourceA;
    public AudioSource audioSourceB;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private Coroutine crossfadeCoroutine;

    private AudioClip lastClip;
    private float lastVolume;
    private float lastFadeIn;

    void Awake()
    {
        Instance = this;

        currentSource = audioSourceA;
        nextSource = audioSourceB;

        audioSourceA.loop = true;
        audioSourceB.loop = true;
        audioSourceA.volume = 0f;
        audioSourceB.volume = 0f;
    }

    public void PlayMusic(AudioClip clip, float fadeOutDuration = 1f, float fadeInDuration = 1f, float targetVolume = 1f)
    {
        lastClip = clip;
        lastVolume = targetVolume;
        lastFadeIn = fadeInDuration;

        if (currentSource.clip == clip && currentSource.isPlaying) return;

        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        crossfadeCoroutine = StartCoroutine(Crossfade(clip, fadeOutDuration, fadeInDuration, targetVolume));
    }

    public void ResumeMusic(float fadeInDuration = 1f)
    {
        if (lastClip == null) return;
        PlayMusic(lastClip, 0f, fadeInDuration, lastVolume);
    }

    IEnumerator Crossfade(AudioClip newClip, float fadeOutDuration, float fadeInDuration, float targetVolume)
    {
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        float currentStartVolume = currentSource.volume;
        float elapsed = 0f;
        float maxDuration = Mathf.Max(fadeOutDuration, fadeInDuration);

        while (elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;

            if (fadeOutDuration > 0f)
                currentSource.volume = Mathf.Lerp(currentStartVolume, 0f,
                    elapsed / fadeOutDuration);

            if (fadeInDuration > 0f)
                nextSource.volume = Mathf.Lerp(0f, targetVolume,
                    elapsed / fadeInDuration);

            yield return null;
        }

        currentSource.volume = 0f;
        currentSource.Stop();
        nextSource.volume = targetVolume;

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }

    public void StopMusic(float fadeOutDuration = 1f)
    {
        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        crossfadeCoroutine = StartCoroutine(FadeOut(fadeOutDuration));
    }

    IEnumerator FadeOut(float duration)
    {
        float startVolume = currentSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        currentSource.volume = 0f;
        currentSource.Stop();
    }
}