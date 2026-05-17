using System.Collections;
using UnityEngine;

public class ClickCounter : MonoBehaviour
{
    public GameObject hiddenItem;
    public int clicksRequired = 3;
    public float bumpScale = 1.2f;
    public float bumpDuration = 0.2f;

    [System.Serializable]
    public class ClickSound
    {
        public AudioClip sound;
        [Range(0f, 1f)]
        public float volume = 1f;
        public float fadeInDuration = 0f;
        public float fadeOutDuration = 0f;
    }

    public ClickSound[] clickSounds;

    [Header("Cooldown")]
    public float clickCooldown = 0.5f;

    [Header("Son en boucle")]
    public AudioClip loopSound;
    [Range(0f, 1f)]
    public float loopSoundVolume = 1f;
    public bool playLoopSound = false;
    public float minDistance = 1f;
    public float maxDistance = 10f;

    private AudioSource audioSource;
    private AudioSource loopAudioSource;
    private HoverParticleManager hoverParticles;
    private Vector3 originalScale;

    private int currentClicks = 0;
    private bool isActivated = false;
    private bool isOnCooldown = false;

    void Start()
    {
        if (hiddenItem != null)
            hiddenItem.SetActive(false);

        hoverParticles = GetComponent<HoverParticleManager>();
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        loopAudioSource = gameObject.AddComponent<AudioSource>();
        loopAudioSource.loop = true;
        loopAudioSource.playOnAwake = false;
        loopAudioSource.spatialBlend = 1f;
        loopAudioSource.rolloffMode = AudioRolloffMode.Linear;
        loopAudioSource.minDistance = minDistance;
        loopAudioSource.maxDistance = maxDistance;
        loopAudioSource.volume = loopSoundVolume;

        if (playLoopSound && loopSound != null)
        {
            loopAudioSource.clip = loopSound;
            loopAudioSource.Play();
        }
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;
        if (isActivated) return;
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    void OnMouseDown()
    {
        if (isActivated) return;
        if (isOnCooldown) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;

        if (clickSounds != null && currentClicks < clickSounds.Length)
        {
            ClickSound cs = clickSounds[currentClicks];
            if (cs.sound != null)
                StartCoroutine(PlayClickSound(cs));
        }

        currentClicks++;
        StartCoroutine(Bump());
        StartCoroutine(ClickCooldown());

        if (currentClicks >= clicksRequired)
        {
            isActivated = true;

            if (loopAudioSource != null && loopAudioSource.isPlaying)
                StartCoroutine(FadeAndStopLoop());

            if (hiddenItem != null)
                hiddenItem.SetActive(true);
            if (hoverParticles != null)
                hoverParticles.Hide();
        }
    }

    IEnumerator PlayClickSound(ClickSound cs)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = cs.sound;
        tempSource.spatialBlend = 0f;
        tempSource.volume = 0f;
        tempSource.Play();

        float elapsed = 0f;
        if (cs.fadeInDuration > 0f)
        {
            while (elapsed < cs.fadeInDuration)
            {
                elapsed += Time.deltaTime;
                tempSource.volume = Mathf.Lerp(0f, cs.volume * SoundSettings.SFXVolume, elapsed / cs.fadeInDuration);
                yield return null;
            }
        }
        tempSource.volume = cs.volume * SoundSettings.SFXVolume; 

        float waitTime = cs.sound.length - cs.fadeInDuration - cs.fadeOutDuration;
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        elapsed = 0f;
        if (cs.fadeOutDuration > 0f)
        {
            while (elapsed < cs.fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                tempSource.volume = Mathf.Lerp(cs.volume * SoundSettings.SFXVolume, 0f, elapsed / cs.fadeOutDuration); 
                yield return null;
            }
        }

        tempSource.volume = 0f;
        Destroy(tempAudio);
    }

    IEnumerator FadeAndStopLoop()
    {
        float fadeDuration = 0.3f;
        float startVolume = loopAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            loopAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        loopAudioSource.Stop();
        loopAudioSource.volume = 0f;
    }

    IEnumerator Bump()
    {
        float elapsed = 0f;
        float halfDuration = bumpDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * bumpScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale * bumpScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    IEnumerator ClickCooldown()
    {
        isOnCooldown = true;
        if (hoverParticles != null)
            hoverParticles.Hide();

        float elapsed = 0f;
        while (elapsed < clickCooldown || audioSource.isPlaying)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        isOnCooldown = false;

        if (!isActivated && hoverParticles != null)
            hoverParticles.Show();
    }
}