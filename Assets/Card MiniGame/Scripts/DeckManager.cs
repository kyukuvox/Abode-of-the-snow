using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    public void InitializeDeck(CardData[] cards)
    {
        drawPile.Clear();
        discardPile.Clear();

        foreach (CardData card in cards)
            drawPile.Add(card);

        Shuffle();
    }

    public CardData DrawCard()
    {
        if (drawPile.Count == 0)
        {
            if (discardPile.Count == 0)
                return null;

            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle();
        }

        CardData card = drawPile[0];
        drawPile.RemoveAt(0);
        return card;
    }

    public void DiscardCard(CardData card)
    {
        if (card != null)
            discardPile.Add(card);
    }

    void Shuffle()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardData temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    public int DrawPileCount() { return drawPile.Count; }
    public int DiscardPileCount() { return discardPile.Count; }
}