using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardData cardData;
    public Image cardImage;
    public Text cardNameText;
    public Text cardDescText;
    public Text cardCostText;

    private bool isPlayable = true;
    private bool isSelectedForDiscard = false;
    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Setup(CardData data)
    {
        cardData = data;
        cardImage.sprite = data.cardSprite;
        cardNameText.text = data.cardName;
        cardDescText.text = data.description;

        switch (data.costType)
        {
            case CardData.CostType.ActionPoints:
                cardCostText.text = data.actionCost + " PA";
                break;
            case CardData.CostType.Life:
                cardCostText.text = data.actionCost + " PV";
                break;
            case CardData.CostType.Defense:
                cardCostText.text = data.actionCost + " DEF";
                break;
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
        if (CardGameManager.Instance.IsDiscardMode())
        {
            CardGameManager.Instance.SelectCardForDiscard(this);
            return;
        }

        if (!isPlayable) return;
        CardGameManager.Instance.PlayCard(this);
    }
}