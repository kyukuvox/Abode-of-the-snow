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

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;
        StartCoroutine(PlaySoundWithFade(clip, volume));
    }

    IEnumerator PlaySoundWithFade(AudioClip clip, float targetVolume)
    {
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
            tempSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / soundFadeDuration);
            yield return null;
        }
        tempSource.volume = targetVolume;

        float waitTime = clip.length - (soundFadeDuration * 2f);
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        elapsed = 0f;
        while (elapsed < soundFadeDuration)
        {
            elapsed += Time.deltaTime;
            tempSource.volume = Mathf.Lerp(targetVolume, 0f, elapsed / soundFadeDuration);
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

            if (hoverParticles != null)
                hoverParticles.Hide();

            if (Inventory.Instance.onItemChangedCallback != null)
                Inventory.Instance.onItemChangedCallback.Invoke();

            PlaySound(activationSound, activationSoundVolume);

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
            Debug.Log("Il vous faut : " + requiredItem.itemName);
        }
    }

    IEnumerator DescendObject()
    {
        PlaySound(descendSound, descendSoundVolume);

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