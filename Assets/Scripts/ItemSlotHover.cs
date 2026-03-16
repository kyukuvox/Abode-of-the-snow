using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverOffset = 40f;  
    public float animationSpeed = 8f; 

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
        basePosition = rectTransform.anchoredPosition;
        targetPosition = basePosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPosition = basePosition + Vector2.up * hoverOffset;
        RestartAnimation();
    }

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