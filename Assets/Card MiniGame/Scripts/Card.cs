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
        cardCostText.text = data.actionCost + " PA";
    }

    public void SetPlayable(bool playable)
    {
        isPlayable = playable;
        // Assombrit la carte si elle n'est pas jouable
        cardImage.color = playable ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
    }

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
        if (!isPlayable) return;
        CardGameManager.Instance.PlayCard(this);
    }
}