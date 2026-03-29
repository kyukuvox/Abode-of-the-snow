using UnityEngine;
using UnityEngine.UI;

public class GlossaireManager : MonoBehaviour
{
    public CardDatabase cardDatabase;

    public Image cardImage;
    public Text cardNameText;
    public Text cardDescriptionText;
    public Text cardStatsText;

    public Button leftArrowButton;
    public Button rightArrowButton;

    private int currentIndex = 0;

    void OnEnable()
    {
        currentIndex = 0;
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
            currentIndex = cardDatabase.allCards.Length - 1;
        DisplayCard(currentIndex);
    }

    void NextCard()
    {
        currentIndex++;
        if (currentIndex >= cardDatabase.allCards.Length)
            currentIndex = 0;
        DisplayCard(currentIndex);
    }

    void DisplayCard(int index)
    {
        CardData card = cardDatabase.allCards[index];

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