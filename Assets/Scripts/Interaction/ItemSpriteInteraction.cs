using System.Collections;
using UnityEngine;

public class ItemSpriteInteraction : MonoBehaviour
{
    public Item requiredItem;
    public GameObject spriteToRemove;
    public bool consumesItem = true;
    public Sprite activatedSprite;
    public float bumpScale = 1.2f;
    public float bumpDuration = 0.2f;

    [Header("Particules à activer")]
    public GameObject particlesToShow;

    [Header("Son")]
    public AudioClip activationLoopSound;
    [Range(0f, 1f)]
    public float loopSoundVolume = 1f;
    public float soundFadeInDuration = 0.3f;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private HoverParticleManager hoverParticles;
    private Vector3 originalScale;
    private AudioSource loopAudioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();
        originalScale = transform.localScale;

        loopAudioSource = gameObject.AddComponent<AudioSource>();
        loopAudioSource.loop = true;
        loopAudioSource.playOnAwake = false;
        loopAudioSource.spatialBlend = 0f;
        loopAudioSource.volume = 0f;

        if (particlesToShow != null)
            particlesToShow.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (isActivated) return;
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    public void TryActivateWithItem(Item item)
    {
        if (isActivated) return;

        if (item == requiredItem)
        {
            isActivated = true;

            if (activatedSprite != null && spriteRenderer != null)
                spriteRenderer.sprite = activatedSprite;

            if (consumesItem)
            {
                if (Inventory.Instance.onItemChangedCallback != null)
                    Inventory.Instance.onItemChangedCallback.Invoke();
            }
            else
                Inventory.Instance.AddItem(item);

            if (spriteToRemove != null)
                Destroy(spriteToRemove);

            if (hoverParticles != null)
                hoverParticles.Hide();

            if (particlesToShow != null)
                particlesToShow.SetActive(true);

            if (activationLoopSound != null && loopAudioSource != null)
            {
                loopAudioSource.clip = activationLoopSound;
                loopAudioSource.volume = 0f;
                loopAudioSource.Play();
                StartCoroutine(FadeInLoop());
            }

            StartCoroutine(Bump());
        }
        else
        {
            Inventory.Instance.AddItem(item);
        }
    }

    IEnumerator FadeInLoop()
    {
        float elapsed = 0f;
        float targetVolume = loopSoundVolume * SoundSettings.SFXVolume;

        while (elapsed < soundFadeInDuration)
        {
            elapsed += Time.deltaTime;
            loopAudioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / soundFadeInDuration);
            yield return null;
        }

        loopAudioSource.volume = targetVolume;
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
}