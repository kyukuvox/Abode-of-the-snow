using System.Collections.Generic;
using UnityEngine;

public class PlayerCardCollection : MonoBehaviour
{
    public static PlayerCardCollection Instance;

    public CardDatabase cardDatabase; 
    private List<CardData> unlockedCards = new List<CardData>(); 

    void Awake()
    {
        Instance = this;

   
        foreach (CardData card in cardDatabase.allCards)
            unlockedCards.Add(card);
    }

    public void AddCard(CardData card)
    {
        if (!unlockedCards.Contains(card))
        {
            unlockedCards.Add(card);
            Debug.Log("Nouvelle carte débloquée : " + card.cardName);
        }
    }

    public List<CardData> GetUnlockedCards()
    {
        return unlockedCards;
    }

    public CardData[] GetUnlockedCardsArray()
    {
        return unlockedCards.ToArray();
    }
}