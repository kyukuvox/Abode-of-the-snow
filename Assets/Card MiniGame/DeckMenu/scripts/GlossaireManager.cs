using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GlossaireManager : MonoBehaviour
{
    public Image cardImage;
    public Text cardNameText;
    public Text cardDescriptionText;
    public Text cardStatsText;
    public Button leftArrowButton;
    public Button rightArrowButton;

    [Header("Base de données")]
    public CardDatabase cardDatabase;

    [Header("Visuel carte non découverte")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Sprite lockedSprite;

    private int currentIndex = 0;
    private CardData[] allCards;

    void Start()
    {
        leftArrowButton.onClick.AddListener(PreviousCard);
        rightArrowButton.onClick.AddListener(NextCard);
    }

    void OnEnable()
    {
        RefreshCards();
    }

    public void RefreshCards()
    {
        if (cardDatabase == null)
        {
            Debug.Log("CardDatabase non assignée !");
            return;
        }

        List<CardData> combined = new List<CardData>();

        if (cardDatabase.rewardCards != null)
            foreach (CardData card in cardDatabase.rewardCards)
                if (!combined.Contains(card))
                    combined.Add(card);

        if (PlayerCardCollection.Instance != null)
        {
            CardData[] unlockedCards = PlayerCardCollection.Instance.GetUnlockedCardsArray();
            foreach (CardData card in unlockedCards)
                if (!combined.Contains(card))
                    combined.Add(card);
        }

        allCards = combined.ToArray();
        Debug.Log("Cartes dans le glossaire : " + allCards.Length);
        currentIndex = 0;

        if (allCards != null && allCards.Length > 0)
            DisplayCard(currentIndex);
        else
            Debug.Log("Aucune carte à afficher !");
    }

    public void PreviousCard()
    {
        if (allCards == null || allCards.Length == 0) return;
        MenuManager.Instance.PlayNavigationSound(); 
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = allCards.Length - 1;
        DisplayCard(currentIndex);
    }

    public void NextCard()
    {
        if (allCards == null || allCards.Length == 0) return;
        MenuManager.Instance.PlayNavigationSound(); 
        currentIndex++;
        if (currentIndex >= allCards.Length)
            currentIndex = 0;
        DisplayCard(currentIndex);
    }

    bool IsCardUnlocked(CardData card)
    {
        if (PlayerCardCollection.Instance == null) return false;
        CardData[] unlockedCards = PlayerCardCollection.Instance.GetUnlockedCardsArray();
        foreach (CardData unlocked in unlockedCards)
            if (unlocked == card) return true;
        return false;
    }

    void DisplayCard(int index)
    {
        CardData card = allCards[index];
        bool unlocked = IsCardUnlocked(card);

        if (unlocked)
        {
            cardImage.sprite = card.cardSprite;
            cardImage.color = Color.white;
            cardNameText.text = card.cardName;
            cardDescriptionText.text = card.description;

            string costLabel = "";
            switch (card.costType)
            {
                case CardData.CostType.ActionPoints: costLabel = card.actionCost + " AP"; break;
                case CardData.CostType.Life: costLabel = card.actionCost + " HP"; break;
                case CardData.CostType.Defense: costLabel = card.actionCost + " DEF"; break;
            }

            cardStatsText.text = "Cost : " + costLabel +
                                 " | Delay : " + card.delayTurns +
                                 " | Power : " + card.power;
        }
        else
        {
            if (lockedSprite != null)
                cardImage.sprite = lockedSprite;
            else
                cardImage.sprite = card.cardSprite;

            cardImage.color = lockedColor;
            cardNameText.text = "???";
            cardDescriptionText.text = "???";
            cardStatsText.text = "Cost : ??? | Delay : ??? | Power : ???";
        }
    }
}