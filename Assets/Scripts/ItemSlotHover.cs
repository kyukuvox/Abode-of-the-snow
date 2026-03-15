using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverOffset = 40f;   // Hauteur de remontée au survol
    public float animationSpeed = 8f; // Vitesse de l'animation

    private Vector2 basePosition;
    private Vector2 targetPosition;
    private RectTransform rectTransform;
    private Coroutine currentAnimation;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Sauvegarde la position de base du slot
        basePosition = rectTransform.anchoredPosition;
        targetPosition = basePosition;
    }

    // Appelé quand la souris entre sur le slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPosition = basePosition + Vector2.up * hoverOffset;
        RestartAnimation();
    }

    // Appelé quand la souris quitte le slot
    public void OnPointerExit(PointerEventData eventData)
    {
        targetPosition = basePosition;
        RestartAnimation();
    }
    public void InitBasePosition()
    {
        basePosition = rectTransform.anchoredPosition;
        targetPosition = basePosition;
    }
    void RestartAnimation()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateSlot());
    }

    IEnumerator AnimateSlot()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * animationSpeed
            );
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
}