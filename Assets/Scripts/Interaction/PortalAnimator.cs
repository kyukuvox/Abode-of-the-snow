using System.Collections;
using UnityEngine;

public class PortalAnimator : MonoBehaviour
{
    public float descendSpeed = 2f;
    public float targetYOffset = -3f;
    public DialogueData triggerDialogue;
    public bool onlyAfterCardGame = false;

    [Header("Particules")]
    public ParticleSystem descendParticles;

    [Header("Caméra")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;
    public float cameraFocusSpeed = 5f;
    public float focusDuration = 2f;

    [Header("Son")]
    public AudioClip shakeSound;
    [Range(0f, 1f)]
    public float shakeSoundVolume = 1f;
    public float soundFadeDuration = 0.3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isActivated = false;
    private AudioSource shakeAudioSource;

    void Awake()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(
            transform.position.x,
            transform.position.y + targetYOffset,
            transform.position.z
        );

        if (descendParticles != null)
            descendParticles.Stop();

        shakeAudioSource = gameObject.AddComponent<AudioSource>();
        shakeAudioSource.loop = true;
        shakeAudioSource.playOnAwake = false;
        shakeAudioSource.spatialBlend = 0f;
        shakeAudioSource.volume = 0f;
    }

    public void TryActivate(DialogueData dialogue)
    {
        if (onlyAfterCardGame) return;
        if (dialogue == triggerDialogue)
            ActivatePortal();
    }

    public void ActivatePortal()
    {
        if (isActivated) return;
        isActivated = true;
        StartCoroutine(DescendPortal());
    }

    IEnumerator DescendPortal()
    {
        CameraGround camGround = Camera.main.GetComponent<CameraGround>();

        if (camGround != null)
        {
            camGround.EnterCinematicMode();
            camGround.SetOverride(true);
        }

        StartCoroutine(FocusCameraOnPortal());

        if (descendParticles != null)
            descendParticles.Play();

        yield return new WaitForSeconds(0.5f);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * descendSpeed
            );
            yield return null;
        }

        transform.position = targetPosition;

        if (shakeSound != null && shakeAudioSource != null)
        {
            shakeAudioSource.clip = shakeSound;
            shakeAudioSource.volume = 0f;
            shakeAudioSource.Play();
            StartCoroutine(FadeAudioSource(shakeAudioSource, 0f, shakeSoundVolume, soundFadeDuration));
        }

        yield return StartCoroutine(ShakeCamera());

        if (shakeAudioSource != null)
            yield return StartCoroutine(FadeAudioSource(shakeAudioSource, shakeSoundVolume, 0f, soundFadeDuration));

        if (shakeAudioSource != null)
            shakeAudioSource.Stop();

        if (descendParticles != null)
            descendParticles.Stop();

        yield return new WaitForSeconds(1f);

        if (camGround != null)
        {
            camGround.ExitCinematicMode();
            camGround.SetOverride(false);
        }
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

    IEnumerator FocusCameraOnPortal()
    {
        Camera cam = Camera.main;
        float elapsed = 0f;

        while (elapsed < focusDuration)
        {
            elapsed += Time.deltaTime;

            Vector3 targetCamPos = new Vector3(
                transform.position.x,
                transform.position.y + 9f,
                -10f
            );

            cam.transform.position = Vector3.Lerp(
                cam.transform.position,
                targetCamPos,
                Time.deltaTime * cameraFocusSpeed
            );

            yield return null;
        }
    }

    IEnumerator ShakeCamera()
    {
        Camera cam = Camera.main;
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            cam.transform.localPosition = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 target = new Vector3(
            transform.position.x,
            transform.position.y + targetYOffset,
            transform.position.z
        );
        Gizmos.DrawWireCube(target, Vector3.one);
        Gizmos.DrawLine(transform.position, target);
    }
}