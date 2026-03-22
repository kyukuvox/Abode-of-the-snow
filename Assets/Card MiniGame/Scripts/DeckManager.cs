using System.Collections.Generic;
using UnityEngine;

public class DeckManager 
{
    private List<CardData> deck = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    public void InitializeDeck(CardData[] cards)
    {
        deck = new List<CardData>(cards);
        ShuffleDeck();
    }

    public CardData DrawCard()
    {
        if (deck.Count == 0)
        {
            if (discardPile.Count == 0) return null;
            deck = new List<CardData>(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        CardData card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public void DiscardCard(CardData card)
    {
        discardPile.Add(card);
    }

    void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            CardData temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    public int DeckCount() { return deck.Count; }
}