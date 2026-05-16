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
    }

    public ClickSound[] clickSounds;
    private AudioSource audioSource;
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
            AudioClip clip = clickSounds[currentClicks].sound;
            float vol = clickSounds[currentClicks].volume;
            if (clip != null)
                audioSource.PlayOneShot(clip, vol);
        }

        currentClicks++;
        StartCoroutine(Bump());
        StartCoroutine(ClickCooldown());

        if (currentClicks >= clicksRequired)
        {
            isActivated = true;
            if (hiddenItem != null)
                hiddenItem.SetActive(true);
            if (hoverParticles != null)
                hoverParticles.Hide();
        }
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

        yield return new WaitUntil(() => !audioSource.isPlaying);
        yield return new WaitForSeconds(0.1f);

        isOnCooldown = false;

        if (!isActivated && hoverParticles != null)
            hoverParticles.Show();
    }
}