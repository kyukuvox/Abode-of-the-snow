using System.Collections;
using UnityEngine;

public class PortalItemActivator : MonoBehaviour
{
    public Item requiredItem;
    public PortalAnimator portalTarget;
    public bool consumesItem = true;
    public Sprite activatedSprite;

    [Header("Sons")]
    public AudioClip activationSound;
    [Range(0f, 1f)]
    public float activationSoundVolume = 1f;
    public float soundFadeDuration = 0.2f;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private HoverParticleManager hoverParticles;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();
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

    IEnumerator PlaySoundWithFade(AudioClip clip, float targetVolume)
    {
        if (clip == null) yield break;
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.spatialBlend = 0f;
        tempSource.volume = 0f;
        tempSource.Play();

        float elapsed = 0f;
        while (elapsed < soundFadeDuration)
        {
            elapsed += Time.deltaTime;
            tempSource.volume = Mathf.Lerp(0f, targetVolume * SoundSettings.SFXVolume, elapsed / soundFadeDuration);
            yield return null;
        }
        tempSource.volume = targetVolume * SoundSettings.SFXVolume;

        float waitTime = clip.length - (soundFadeDuration * 2f);
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        elapsed = 0f;
        while (elapsed < soundFadeDuration)
        {
            elapsed += Time.deltaTime;
            tempSource.volume = Mathf.Lerp(targetVolume * SoundSettings.SFXVolume, 0f, elapsed / soundFadeDuration);
            yield return null;
        }

        tempSource.volume = 0f;
        Destroy(tempAudio);
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

            if (activationSound != null)
                StartCoroutine(PlaySoundWithFade(activationSound, activationSoundVolume));

            if (portalTarget != null)
                portalTarget.ActivatePortal();

            if (hoverParticles != null)
                hoverParticles.Hide();
        }
        else
        {
            Inventory.Instance.AddItem(item);
        }
    }
}