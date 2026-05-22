using System.Collections;
using UnityEngine;

public class ItemInteractableSprite : MonoBehaviour
{
    public Item requiredItem;
    public Sprite activatedSprite;

    public enum ActivationMode { SpriteChange, Animation }

    [Header("Mode activation")]
    public ActivationMode activationMode = ActivationMode.SpriteChange;

    [Header("Mode SpriteChange")]
    public GameObject animatedObject;
    public float targetYOffset = -3f;
    public float descendSpeed = 2f;

    [Header("Mode Animation")]
    public Animator targetAnimator;
    public string animationTrigger = "Activate";

    [Header("Sons")]
    public AudioClip activationSound;
    [Range(0f, 1f)]
    public float activationSoundVolume = 1f;
    public AudioClip descendSound;
    [Range(0f, 1f)]
    public float descendSoundVolume = 1f;
    public float soundFadeDuration = 0.2f;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private HoverParticleManager hoverParticles;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();

        if (ActivatedObjectsTracker.Instance != null &&
            ActivatedObjectsTracker.Instance.IsActivated(gameObject.name))
        {
            isActivated = true;
            if (activatedSprite != null && spriteRenderer != null)
                spriteRenderer.sprite = activatedSprite;
            if (hoverParticles != null)
                hoverParticles.Hide();
            if (animatedObject != null)
            {
                Vector3 targetPosition = new Vector3(
                    animatedObject.transform.position.x,
                    animatedObject.transform.position.y + targetYOffset,
                    animatedObject.transform.position.z
                );
                animatedObject.transform.position = targetPosition;
            }
        }
    }

    public void ForceActivate()
    {
        isActivated = true;

        if (hoverParticles != null)
            hoverParticles.Hide();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (activationMode == ActivationMode.Animation && targetAnimator != null)
        {
            targetAnimator.enabled = false;
        }

        if (activatedSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = activatedSprite;

        if (animatedObject != null)
        {
            Vector3 targetPosition = new Vector3(
                animatedObject.transform.position.x,
                animatedObject.transform.position.y + targetYOffset,
                animatedObject.transform.position.z
            );
            animatedObject.transform.position = targetPosition;
        }
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

            if (ActivatedObjectsTracker.Instance != null)
                ActivatedObjectsTracker.Instance.RegisterActivated(gameObject.name);

            if (activatedSprite != null && spriteRenderer != null)
                spriteRenderer.sprite = activatedSprite;

            if (hoverParticles != null)
                hoverParticles.Hide();

            if (Inventory.Instance.onItemChangedCallback != null)
                Inventory.Instance.onItemChangedCallback.Invoke();

            StartCoroutine(PlaySoundWithFade(activationSound, activationSoundVolume));

            switch (activationMode)
            {
                case ActivationMode.SpriteChange:
                    if (animatedObject != null)
                        StartCoroutine(DescendObject());
                    break;

                case ActivationMode.Animation:
                    if (targetAnimator != null)
                        targetAnimator.SetTrigger(animationTrigger);
                    if (animatedObject != null)
                        StartCoroutine(DescendObject());
                    break;
            }
        }
        else
        {
            Inventory.Instance.AddItem(item);
        }
    }

    IEnumerator DescendObject()
    {
        StartCoroutine(PlaySoundWithFade(descendSound, descendSoundVolume));

        Vector3 targetPosition = new Vector3(
            animatedObject.transform.position.x,
            animatedObject.transform.position.y + targetYOffset,
            animatedObject.transform.position.z
        );

        while (Vector3.Distance(animatedObject.transform.position, targetPosition) > 0.01f)
        {
            animatedObject.transform.position = Vector3.Lerp(
                animatedObject.transform.position,
                targetPosition,
                Time.deltaTime * descendSpeed
            );
            yield return null;
        }

        animatedObject.transform.position = targetPosition;
    }

    void OnDrawGizmosSelected()
    {
        if (activationMode == ActivationMode.SpriteChange && animatedObject != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 target = new Vector3(
                animatedObject.transform.position.x,
                animatedObject.transform.position.y + targetYOffset,
                animatedObject.transform.position.z
            );
            Gizmos.DrawWireCube(target, Vector3.one * 0.5f);
            Gizmos.DrawLine(animatedObject.transform.position, target);
        }
    }
}