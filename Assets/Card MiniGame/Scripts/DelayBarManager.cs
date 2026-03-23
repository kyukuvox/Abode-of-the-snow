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

        Text cardNameText = entry.transform.Find("CardNameText").GetComponent<Text>();
        Text turnsLeftText = entry.transform.Find("TurnsLeftText").GetComponent<Text>();
        Text ownerText = entry.transform.Find("OwnerText").GetComponent<Text>();
        Image background = entry.GetComponent<Image>();

        cardNameText.text = card.cardName;
        turnsLeftText.text = "Dans " + turns + " tours";
        ownerText.text = isPlayer ? "J" : "E";

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
    }

    public void ResetDelayBar()
    {
        foreach (DelayedCard delayed in delayedCards)
            if (delayed.uiElement != null)
                Destroy(delayed.uiElement);

        delayedCards.Clear();
    }
}