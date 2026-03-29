using UnityEngine;
using UnityEngine.UI;

public class GlossaireManager : MonoBehaviour
{
    public Image cardImage;
    public Text cardNameText;
    public Text cardDescriptionText;
    public Text cardStatsText;

    public Button leftArrowButton;
    public Button rightArrowButton;

    private int currentIndex = 0;
    private CardData[] currentCards;

    void OnEnable()
    {
        currentCards = PlayerCardCollection.Instance.GetUnlockedCardsArray();
        currentIndex = 0;

        if (currentCards.Length > 0)
            DisplayCard(currentIndex);
    }

    void Start()
    {
        leftArrowButton.onClick.AddListener(PreviousCard);
        rightArrowButton.onClick.AddListener(NextCard);
    }

    void PreviousCard()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = currentCards.Length - 1;
        DisplayCard(currentIndex);
    }

    void NextCard()
    {
        currentIndex++;
        if (currentIndex >= currentCards.Length)
            currentIndex = 0;
        DisplayCard(currentIndex);
    }

    void DisplayCard(int index)
    {
        CardData card = currentCards[index];

        cardImage.sprite = card.cardSprite;
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.description;

        string costLabel = "";
        switch (card.costType)
        {
            case CardData.CostType.ActionPoints: costLabel = card.actionCost + " PA"; break;
            case CardData.CostType.Life: costLabel = card.actionCost + " PV"; break;
            case CardData.CostType.Defense: costLabel = card.actionCost + " DEF"; break;
        }

        cardStatsText.text = "Coût : " + costLabel +
                             " | Délai : " + card.delayTurns +
                             " | Puissance : " + card.power;
    }
}