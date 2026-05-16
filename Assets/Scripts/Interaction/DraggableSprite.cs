using System.Collections;
using UnityEngine;

public class DraggableSprite : MonoBehaviour
{
    public GameObject hiddenItem;
    public float triggerDistance = 2f;
    public float maxPullDistance = 1.5f;
    public float resistanceStrength = 3f;

    [Header("Sons")]
    public AudioClip dragSound;
    [Range(0f, 1f)]
    public float dragSoundVolume = 1f;
    public AudioClip activationSound;
    [Range(0f, 1f)]
    public float activationSoundVolume = 1f;
    public float soundFadeDuration = 0.2f;

    private bool isDragging = false;
    private bool isActivated = false;
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private HoverParticleManager hoverParticles;
    private AudioSource dragAudioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();
        startPosition = transform.position;

        if (hiddenItem != null)
            hiddenItem.SetActive(false);

        dragAudioSource = gameObject.AddComponent<AudioSource>();
        dragAudioSource.clip = dragSound;
        dragAudioSource.loop = true;
        dragAudioSource.playOnAwake = false;
        dragAudioSource.spatialBlend = 0f;
        dragAudioSource.volume = 0f;
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
        if (!isDragging)
            if (hoverParticles != null)
                hoverParticles.Hide();
    }

    void OnMouseDown()
    {
        if (isActivated) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;

        isDragging = true;

        if (dragSound != null && dragAudioSource != null)
        {
            dragAudioSource.volume = 0f;
            dragAudioSource.Play();
            StartCoroutine(FadeAudioSource(dragAudioSource, 0f, dragSoundVolume, soundFadeDuration));
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 direction = mousePosition - startPosition;
        float distance = direction.magnitude;

        if (distance > maxPullDistance)
        {
            float resistedDistance = maxPullDistance * (1f - 1f / (1f + distance / resistanceStrength));
            transform.position = startPosition + direction.normalized * resistedDistance;
        }
        else
        {
            transform.position = startPosition + direction;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float maxAngle = 15f;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        if (dragAudioSource != null)
            StartCoroutine(FadeAndStopAudioSource(dragAudioSource, dragSoundVolume, 0f, soundFadeDuration));

        float distance = Vector3.Distance(startPosition, transform.position);

        if (distance >= triggerDistance && !isActivated)
        {
            isActivated = true;

            if (hiddenItem != null)
                hiddenItem.SetActive(true);
            if (hoverParticles != null)
                hoverParticles.Hide();
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            if (activationSound != null)
                StartCoroutine(PlaySoundWithFade(activationSound, activationSoundVolume));
        }

        StartCoroutine(ReturnToStart());
    }

    IEnumerator FadeAudioSource(AudioSource source, float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;
        source.volume = startVolume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        source.volume = endVolume;
    }

    IEnumerator FadeAndStopAudioSource(AudioSource source, float startVolume, float endVolume, float duration)
    {
        yield return StartCoroutine(FadeAudioSource(source, startVolume, endVolume, duration));
        source.Stop();
        source.volume = 0f;
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

    IEnumerator ReturnToStart()
    {
        float elapsed = 0f;
        float duration = 0.3f;
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(currentPos, startPosition, t);
            transform.rotation = Quaternion.Lerp(currentRot, Quaternion.identity, t);
            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxPullDistance);
    }
}