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

    private bool isPlayable = true;
    private bool isSelectedForDiscard = false;
    private Vector3 baseScale;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;

    void Awake()
    {
        baseScale = transform.localScale;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

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
        transform.localScale = baseScale * 1.15f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = baseScale;
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
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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

        transform.localScale = baseScale;
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
        // Sauvegarde la position écran avant tout changement
        Vector3 startWorldPos = rectTransform.position;
        Vector3 targetWorldPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // Restaure la position monde après changement de parent
        rectTransform.position = startWorldPos;

        Vector3 startScale = transform.localScale;

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Utilise position monde au lieu de anchoredPosition
            rectTransform.position = Vector3.Lerp(startWorldPos, targetWorldPos, t);
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.2f, t);

            if (t > 0.5f)
                cg.alpha = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);

            yield return null;
        }

        onComplete?.Invoke();
    }
}