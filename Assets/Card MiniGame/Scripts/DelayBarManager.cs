using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayBarManager : MonoBehaviour
{
    public static DelayBarManager Instance;

    public Transform delayContent;
    public GameObject delayCardPrefab;
    public Sprite playerCardBackground;
    public Sprite enemyCardBackground;

    private const float CARD_WIDTH = 468f;
    private const float CARD_HEIGHT = 240f;
    private const float CARD_SPACING = 10f;

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

        entryRect.sizeDelta = new Vector2(CARD_WIDTH, CARD_HEIGHT);
        entryRect.anchorMin = new Vector2(0, 0.5f);
        entryRect.anchorMax = new Vector2(0, 0.5f);
        entryRect.pivot = new Vector2(0, 0.5f);

        float xPos = delayedCards.Count * (CARD_WIDTH + CARD_SPACING);
        entryRect.anchoredPosition = new Vector2(xPos, 0f);

        RectTransform contentRect = delayContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(
            (delayedCards.Count + 1) * (CARD_WIDTH + CARD_SPACING),
            CARD_HEIGHT
        );

        Text cardNameText = entry.transform.Find("CardNameText").GetComponent<Text>();
        Text turnsLeftText = entry.transform.Find("TurnsLeftText").GetComponent<Text>();
        Text ownerText = entry.transform.Find("OwnerText").GetComponent<Text>();
        Image background = entry.GetComponent<Image>();

        if (cardNameText != null) cardNameText.text = card.cardName;
        if (turnsLeftText != null) turnsLeftText.text = "Dans " + turns + " tours";
        if (ownerText != null) ownerText.text = isPlayer ? "J" : "E";

        if (background != null)
        {
            if (isPlayer && playerCardBackground != null)
            {
                background.sprite = playerCardBackground;
                background.color = Color.white;
            }
            else if (!isPlayer && enemyCardBackground != null)
            {
                background.sprite = enemyCardBackground;
                background.color = Color.white;
            }
            else
            {
                background.color = isPlayer ?
                    new Color(0.2f, 0.5f, 1f, 0.8f) :
                    new Color(1f, 0.3f, 0.3f, 0.8f);
            }
        }

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
        if (delayedCards.Count == 0)
        {
            RectTransform contentRect = delayContent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(0f, CARD_HEIGHT);
            return;
        }

        for (int i = 0; i < delayedCards.Count; i++)
        {
            RectTransform rect = delayedCards[i].uiElement.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(CARD_WIDTH, CARD_HEIGHT);
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(i * (CARD_WIDTH + CARD_SPACING), 0f);
        }

        RectTransform contentRectUpdate = delayContent.GetComponent<RectTransform>();
        contentRectUpdate.sizeDelta = new Vector2(
            delayedCards.Count * (CARD_WIDTH + CARD_SPACING),
            CARD_HEIGHT
        );
    }

    public void ResetDelayBar()
    {
        foreach (DelayedCard delayed in delayedCards)
            if (delayed.uiElement != null)
                Destroy(delayed.uiElement);

        delayedCards.Clear();

        RectTransform contentRect = delayContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(0f, CARD_HEIGHT);
    }
}