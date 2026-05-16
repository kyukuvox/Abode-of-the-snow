using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemSlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverOffset = 10f;
    public float animationSpeed = 10f;

    [Header("Son")]
    public AudioClip hoverSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    public static bool IsDragging = false;

    private RectTransform visualRect;
    private Vector2 basePosition;
    private Vector2 targetPosition;
    private Coroutine animCoroutine;
    private bool isReady = false;
    private AudioSource audioSource;

    void Start()
    {
        Transform visual = transform.Find("Visual");
        if (visual != null)
            visualRect = visual.GetComponent<RectTransform>();
        else
            visualRect = GetComponent<RectTransform>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        StartCoroutine(InitBasePosition());
    }

    IEnumerator InitBasePosition()
    {
        yield return null;
        yield return null;
        yield return null;

        basePosition = visualRect.anchoredPosition;
        targetPosition = basePosition;
        isReady = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isReady) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (IsDragging) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;

        targetPosition = basePosition + Vector2.up * hoverOffset;
        RestartAnimation();

        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound, soundVolume);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isReady) return;
        targetPosition = basePosition;
        RestartAnimation();
    }

    void RestartAnimation()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateToTarget());
    }

    IEnumerator AnimateToTarget()
    {
        while (Vector2.Distance(visualRect.anchoredPosition, targetPosition) > 0.1f)
        {
            visualRect.anchoredPosition = Vector2.Lerp(
                visualRect.anchoredPosition,
                targetPosition,
                Time.deltaTime * animationSpeed
            );
            yield return null;
        }
        visualRect.anchoredPosition = targetPosition;
    }
}