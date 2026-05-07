using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayBarManager : MonoBehaviour
{
    public static DelayBarManager Instance;

    public Transform delayContent;
    public GameObject delayCardPrefab;

    private class DelayedCard
    {
        public CardData card;
        public int turnsLeft;
        public bool isPlayer;
        public GameObject uiElement;
        public Text turnsLeftText;
    }

    private List<DelayedCard> delayedCards = new List<DelayedCard>();

    void Awake()
    {
        Instance = this;
    }

    public void AddDelayedCard(CardData card, int turns, bool isPlayer)
    {
        GameObject entry = Instantiate(delayCardPrefab, delayContent);

        RectTransform entryRect = entry.GetComponent<RectTransform>();
        float cardWidth = entryRect.sizeDelta.x;
        float cardHeight = entryRect.sizeDelta.y;
        float spacing = 10f;

        entryRect.anchorMin = new Vector2(0, 0.5f);
        entryRect.anchorMax = new Vector2(0, 0.5f);
        entryRect.pivot = new Vector2(0, 0.5f);

        float xPos = delayedCards.Count * (cardWidth + spacing);
        entryRect.anchoredPosition = new Vector2(xPos, 0f);

        entryRect.sizeDelta = new Vector2(cardWidth, cardHeight);

        RectTransform contentRect = delayContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(
            (delayedCards.Count + 1) * (cardWidth + spacing),
            contentRect.sizeDelta.y
        );

        Text cardNameText = entry.transform.Find("CardNameText").GetComponent<Text>();
        Text turnsLeftText = entry.transform.Find("TurnsLeftText").GetComponent<Text>();
        Text ownerText = entry.transform.Find("OwnerText").GetComponent<Text>();
        Image background = entry.GetComponent<Image>();

        if (cardNameText != null) cardNameText.text = card.cardName;
        if (turnsLeftText != null) turnsLeftText.text = "Dans " + turns + " tours";
        if (ownerText != null) ownerText.text = isPlayer ? "J" : "E";

        if (background != null)
            background.color = isPlayer ?
                new Color(0.2f, 0.5f, 1f, 0.8f) :
                new Color(1f, 0.3f, 0.3f, 0.8f);

        DelayedCard delayed = new DelayedCard
        {
            card = card,
            turnsLeft = turns,
            isPlayer = isPlayer,
            uiElement = entry,
            turnsLeftText = turnsLeftText
        };

        delayedCards.Add(delayed);
    }

    public void TickTurn(bool isPlayerTurn)
    {
        List<DelayedCard> toRemove = new List<DelayedCard>();

        foreach (DelayedCard delayed in delayedCards)
        {
            if (delayed.isPlayer == isPlayerTurn)
            {
                delayed.turnsLeft--;

                if (delayed.turnsLeftText != null)
                    delayed.turnsLeftText.text = delayed.turnsLeft > 0 ?
                        "Dans " + delayed.turnsLeft + " tours" :
                        "Ce tour !";

                if (delayed.turnsLeft <= 0)
                {
                    CardGameManager.Instance.ApplyDelayedCard(delayed.card, delayed.isPlayer);
                    Destroy(delayed.uiElement);
                    toRemove.Add(delayed);
                }
            }
        }

        foreach (DelayedCard d in toRemove)
            delayedCards.Remove(d);

        RefreshCardPositions();
    }

    void RefreshCardPositions()
    {
        if (delayedCards.Count == 0) return;

        float cardWidth = delayedCards[0].uiElement.GetComponent<RectTransform>().sizeDelta.x;
        float spacing = 10f;

        for (int i = 0; i < delayedCards.Count; i++)
        {
            RectTransform rect = delayedCards[i].uiElement.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(i * (cardWidth + spacing), 0f);
        }

        RectTransform contentRect = delayContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(
            delayedCards.Count * (cardWidth + spacing),
            contentRect.sizeDelta.y
        );
    }

    public void ResetDelayBar()
    {
        foreach (DelayedCard delayed in delayedCards)
            if (delayed.uiElement != null)
                Destroy(delayed.uiElement);

        delayedCards.Clear();

        RectTransform contentRect = delayContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(0f, contentRect.sizeDelta.y);
    }
}