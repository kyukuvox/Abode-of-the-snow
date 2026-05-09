using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData cardData;
    public Image cardImage;
    public Text cardNameText;
    public Text cardDescText;
    public Text cardCostText;

    public float tiltStrength = 15f;
    public float tiltSmoothing = 8f;

    private bool isPlayable = true;
    private bool isSelectedForDiscard = false;
    private bool isDragging = false;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;

    private Vector2 previousMousePos;
    private float currentTilt = 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void InitRestPosition() { }

    public void Setup(CardData data)
    {
        cardData = data;
        cardImage.sprite = data.cardSprite;
        cardNameText.text = data.cardName;
        cardDescText.text = data.description;

        switch (data.costType)
        {
            case CardData.CostType.ActionPoints: cardCostText.text = data.actionCost + " PA"; break;
            case CardData.CostType.Life: cardCostText.text = data.actionCost + " PV"; break;
            case CardData.CostType.Defense: cardCostText.text = data.actionCost + " DEF"; break;
        }
    }

    public void SetPlayable(bool playable)
    {
        isPlayable = playable;
        if (!isSelectedForDiscard)
            cardImage.color = playable ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public void ToggleDiscardSelection()
    {
        isSelectedForDiscard = !isSelectedForDiscard;
        cardImage.color = isSelectedForDiscard ? new Color(1f, 0.8f, 0f, 1f) : Color.white;
    }

    public void ResetDiscardSelection()
    {
        isSelectedForDiscard = false;
        cardImage.color = Color.white;
    }

    public bool IsSelectedForDiscard() { return isSelectedForDiscard; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
            transform.localScale = Vector3.one * 1.15f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
            transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging) return;

        if (CardGameManager.Instance.IsDiscardMode())
        {
            CardGameManager.Instance.SelectCardForDiscard(this);
            return;
        }

        if (!isPlayable) return;
        CardGameManager.Instance.PlayCard(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        transform.localScale = Vector3.one * 1.1f;

        canvasGroup.alpha = 0.9f;
        canvasGroup.blocksRaycasts = false;

        previousMousePos = eventData.position;
        currentTilt = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        Vector2 mouseDelta = eventData.position - previousMousePos;
        previousMousePos = eventData.position;

        float targetTilt = Mathf.Clamp(-mouseDelta.x * tiltStrength * 0.1f, -tiltStrength, tiltStrength);
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmoothing);
        transform.rotation = Quaternion.Euler(0, 0, currentTilt);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Card targetCard = GetCardUnderCursor(eventData);

        if (targetCard != null && targetCard != this)
        {
            int targetIndex = targetCard.transform.GetSiblingIndex();
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(targetIndex);
            CardGameManager.Instance.SwapCards(this, targetCard);
        }
        else
        {
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalSiblingIndex);
            rectTransform.anchoredPosition = originalPosition;
        }

        transform.localScale = Vector3.one;
        StartCoroutine(ResetTilt());
    }

    IEnumerator ResetTilt()
    {
        while (Mathf.Abs(currentTilt) > 0.1f)
        {
            currentTilt = Mathf.Lerp(currentTilt, 0f, Time.deltaTime * tiltSmoothing);
            transform.rotation = Quaternion.Euler(0, 0, currentTilt);
            yield return null;
        }
        currentTilt = 0f;
        transform.rotation = Quaternion.identity;
    }

    Card GetCardUnderCursor(PointerEventData eventData)
    {
        foreach (GameObject obj in eventData.hovered)
        {
            Card card = obj.GetComponent<Card>();
            if (card != null && card != this)
                return card;
        }
        return null;
    }

    public void PlayAnimation(System.Action onComplete)
    {
        StartCoroutine(PlayCardAnimation(onComplete));
    }

    IEnumerator PlayCardAnimation(System.Action onComplete)
    {
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        Vector3 startWorldPos = rectTransform.position;
        Vector3 targetWorldPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 startScale = transform.localScale;

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rectTransform.position = Vector3.Lerp(startWorldPos, targetWorldPos, t);
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.2f, t);

            if (t > 0.5f)
                cg.alpha = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);

            yield return null;
        }

        onComplete?.Invoke();
    }
}