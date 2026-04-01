using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance;

    [Header("Canvas")]
    public GameObject cardGameCanvas;

    [Header("UI Joueur")]
    public Text playerLifeText;
    public Text playerActionText;
    public Text playerDefenseText;
    public Transform playerHandZone;

    [Header("UI Ennemi")]
    public Text enemyLifeText;
    public Text enemyActionText;
    public Text enemyDefenseText;
    public Image enemyCardSprite;

    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("Résultat")]
    public GameObject resultPanel;
    public Text resultText;

    [Header("Boutons")]
    public Button endTurnButton;
    public Button discardButton;

    [Header("Deck Builder")]
    public DeckBuilderManager deckBuilderManager;

    private int playerLife;
    private int playerActionPoints;
    private int playerDefense;

    private int enemyLife;
    private int enemyActionPoints;
    private int enemyDefense;

    private CharacterCardData enemyData;
    private CharacterCardData playerData;

    private DeckManager playerDeck = new DeckManager();
    private DeckManager enemyDeck = new DeckManager();

    private List<Card> playerHand = new List<Card>();
    private List<Card> selectedForDiscard = new List<Card>();

    private bool isDiscardMode = false;
    private bool isPlayerTurn = true;
    private bool hasDiscardedThisTurn = false;

    public float shakeIntensity = 5f;
    public float shakeDuration = 0.3f;

    public float resultAnimationSpeed = 5f;
    public float resultSlideOffset = 50f;

    private Item rewardItem;
    private NPCWithItemDialogue currentNPC;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        discardButton.onClick.AddListener(ToggleDiscardMode);
        Button closeButton = resultPanel.GetComponentInChildren<Button>();
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCardGame);
    }

    public void StartCardGame(CharacterCardData enemy, CharacterCardData player, Item reward, NPCWithItemDialogue npc)
    {
        Debug.Log("=== START CARD GAME ===");
        Debug.Log("cardGameCanvas : " + (cardGameCanvas != null ? "OK" : "NULL"));
        Debug.Log("enemy : " + (enemy != null ? enemy.characterName : "NULL"));
        Debug.Log("player : " + (player != null ? player.characterName : "NULL"));
        Debug.Log("cardPrefab : " + (cardPrefab != null ? "OK" : "NULL"));
        Debug.Log("playerHandZone : " + (playerHandZone != null ? "OK" : "NULL"));

        currentNPC = npc;
        enemyData = enemy;
        playerData = player;
        rewardItem = reward;

        playerLife = player.maxLife;
        playerActionPoints = player.actionPointsPerTurn;
        playerDefense = 0;

        enemyLife = enemy.maxLife;
        enemyActionPoints = enemy.actionPointsPerTurn;
        enemyDefense = 0;

        List<CardData> playerDeckCards = deckBuilderManager != null ?
            deckBuilderManager.GetCurrentDeck() : new List<CardData>();

        Debug.Log("Cartes dans le DeckBuilder : " + playerDeckCards.Count);

        if (playerDeckCards.Count > 0)
            playerDeck.InitializeDeck(playerDeckCards.ToArray());
        else
            playerDeck.InitializeDeck(player.startingDeck);

        enemyDeck.InitializeDeck(enemy.startingDeck);

        cardGameCanvas.SetActive(true);
        resultPanel.SetActive(false);

        enemyCardSprite.sprite = enemy.characterSprite;

        for (int i = 0; i < 5; i++)
            DrawPlayerCard();

        UpdateUI();
        isPlayerTurn = true;
    }

    IEnumerator ShakeText(Text text)
    {
        Vector3 originalPos = text.rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPos.y + Random.Range(-shakeIntensity, shakeIntensity);
            text.rectTransform.anchoredPosition = new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        text.rectTransform.anchoredPosition = originalPos;
    }

    public void SwapCards(Card cardA, Card cardB)
    {
        int indexA = playerHand.IndexOf(cardA);
        int indexB = playerHand.IndexOf(cardB);

        if (indexA == -1 || indexB == -1) return;

        playerHand[indexA] = cardB;
        playerHand[indexB] = cardA;
    }

    public void PlayCard(Card card)
    {
        if (!isPlayerTurn) return;

        switch (card.cardData.costType)
        {
            case CardData.CostType.ActionPoints:
                if (playerActionPoints < card.cardData.actionCost) return;
                playerActionPoints -= card.cardData.actionCost;
                break;
            case CardData.CostType.Life:
                if (playerLife <= card.cardData.actionCost) return;
                playerLife -= card.cardData.actionCost;
                break;
            case CardData.CostType.Defense:
                if (playerDefense < card.cardData.actionCost) return;
                playerDefense -= card.cardData.actionCost;
                break;
        }

        playerHand.Remove(card);
        playerDeck.DiscardCard(card.cardData);

        CardData playedCardData = card.cardData;

        card.PlayAnimation(() =>
        {
            if (playedCardData.delayTurns > 0)
                DelayBarManager.Instance.AddDelayedCard(playedCardData, playedCardData.delayTurns, true);
            else
                ApplyCardEffect(playedCardData, true);

            Destroy(card.gameObject);
            UpdateUI();
            CheckGameOver();
        });
    }

    void ApplyCardEffect(CardData card, bool isPlayer)
    {
        if (isPlayer)
        {
            switch (card.cardType)
            {
                case CardData.CardType.Attack:
                    ApplyAttack(card, ref enemyDefense, ref enemyActionPoints, ref enemyLife, true); 
                    break;
                case CardData.CardType.Defense:
                    playerDefense += card.power;
                    break;
                case CardData.CardType.Recharge:
                    playerActionPoints = Mathf.Min(
                        playerActionPoints + card.power,
                        playerData.maxActionPoints
                    );
                    break;
            }
        }
        else
        {
            switch (card.cardType)
            {
                case CardData.CardType.Attack:
                    ApplyAttack(card, ref playerDefense, ref playerActionPoints, ref playerLife, false);
                    break;
                case CardData.CardType.Defense:
                    enemyDefense += card.power;
                    break;
                case CardData.CardType.Recharge:
                    enemyActionPoints = Mathf.Min(
                        enemyActionPoints + card.power,
                        enemyData.maxActionPoints
                    );
                    break;
            }
        }
    }

    void ApplyAttack(CardData card, ref int defense, ref int actionPoints, ref int life, bool isPlayer)
    {
        switch (card.attackType)
        {
            case CardData.AttackType.HitDefense:
                defense = Mathf.Max(0, defense - card.power);
                StartCoroutine(ShakeText(isPlayer ? enemyDefenseText : playerDefenseText));
                break;

            case CardData.AttackType.HitRecharge:
                actionPoints = Mathf.Max(0, actionPoints - card.power);
                StartCoroutine(ShakeText(isPlayer ? enemyActionText : playerActionText));
                break;

            case CardData.AttackType.HitLife:
                int oldDefense = defense;
                int remaining = card.power - defense;
                defense = Mathf.Max(0, defense - card.power);

                if (oldDefense > 0)
                    StartCoroutine(ShakeText(isPlayer ? enemyDefenseText : playerDefenseText));

                if (remaining > 0)
                {
                    life -= remaining;
                    life = Mathf.Max(0, life); 
                    StartCoroutine(ShakeText(isPlayer ? enemyLifeText : playerLifeText));
                }
                break;
        }
    }

    void DrawPlayerCard()
    {
        CardData data = playerDeck.DrawCard();
        if (data == null) return;

        GameObject cardObj = Instantiate(cardPrefab, playerHandZone);
        Card card = cardObj.GetComponent<Card>();
        card.Setup(data);
        playerHand.Add(card);
    }

    public bool IsDiscardMode() { return isDiscardMode; }

    public void ToggleDiscardMode()
    {
        if (!isDiscardMode && hasDiscardedThisTurn) return;

        isDiscardMode = !isDiscardMode;

        if (isDiscardMode)
        {
            discardButton.GetComponentInChildren<Text>().text = "Confirmer (0/2)";
            selectedForDiscard.Clear();
        }
        else
        {
            if (selectedForDiscard.Count == 2)
                ConfirmDiscard();
            else
            {
                foreach (Card card in playerHand)
                    card.ResetDiscardSelection();
                selectedForDiscard.Clear();
                discardButton.GetComponentInChildren<Text>().text = "Défausser";
            }
        }
    }

    public void SelectCardForDiscard(Card card)
    {
        if (!isDiscardMode) return;

        if (selectedForDiscard.Contains(card))
        {
            selectedForDiscard.Remove(card);
            card.ToggleDiscardSelection();
        }
        else if (selectedForDiscard.Count < 2)
        {
            selectedForDiscard.Add(card);
            card.ToggleDiscardSelection();
        }

        discardButton.GetComponentInChildren<Text>().text =
            "Confirmer (" + selectedForDiscard.Count + "/2)";
    }

    void ConfirmDiscard()
    {
        foreach (Card card in selectedForDiscard)
        {
            playerDeck.DiscardCard(card.cardData);
            playerHand.Remove(card);
            Destroy(card.gameObject);
        }

        selectedForDiscard.Clear();

        for (int i = 0; i < 2; i++)
            DrawPlayerCard();

        isDiscardMode = false;
        hasDiscardedThisTurn = true;
        discardButton.interactable = false;
        discardButton.GetComponentInChildren<Text>().text = "Défaussé";
        UpdateUI();
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;
        isPlayerTurn = false;
        endTurnButton.interactable = false;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        DelayBarManager.Instance.TickTurn(false);

        enemyActionPoints = enemyData.actionPointsPerTurn;

        CardData[] hand = new CardData[3];
        for (int i = 0; i < 3; i++)
            hand[i] = enemyDeck.DrawCard();

        foreach (CardData card in hand)
        {
            if (card == null) continue;

            if (enemyActionPoints >= card.actionCost)
            {
                yield return new WaitForSeconds(0.8f);
                enemyActionPoints -= card.actionCost;

                if (card.delayTurns > 0)
                    DelayBarManager.Instance.AddDelayedCard(card, card.delayTurns, false);
                else
                    ApplyCardEffect(card, false);

                enemyDeck.DiscardCard(card);
                UpdateUI();

                if (CheckGameOver()) yield break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        isPlayerTurn = true;
        endTurnButton.interactable = true;
        playerActionPoints = playerData.actionPointsPerTurn;
        hasDiscardedThisTurn = false;
        discardButton.interactable = true;
        discardButton.GetComponentInChildren<Text>().text = "Défausser";

        DelayBarManager.Instance.TickTurn(true);

        while (playerHand.Count < 5)
            DrawPlayerCard();

        foreach (Card card in playerHand)
        {
            bool canPlay = false;
            switch (card.cardData.costType)
            {
                case CardData.CostType.ActionPoints:
                    canPlay = playerActionPoints >= card.cardData.actionCost;
                    break;
                case CardData.CostType.Life:
                    canPlay = playerLife > card.cardData.actionCost;
                    break;
                case CardData.CostType.Defense:
                    canPlay = playerDefense >= card.cardData.actionCost;
                    break;
            }
            card.SetPlayable(canPlay);
        }

        UpdateUI();
    }

    bool CheckGameOver()
    {
        if (enemyLife <= 0)
        {
            EndGame(true);
            return true;
        }
        if (playerLife <= 0)
        {
            EndGame(false);
            return true;
        }
        return false;
    }

    void EndGame(bool playerWon)
    {
        resultText.text = playerWon ? "Victoire !" : "Défaite...";

        if (currentNPC != null)
            currentNPC.SetDefeated(playerWon);

        if (playerWon)
        {
            if (rewardItem != null)
                Inventory.Instance.AddItem(rewardItem);

            if (enemyData.rewardCard != null)
                PlayerCardCollection.Instance.AddCard(enemyData.rewardCard);
        }

        StartCoroutine(AnimateResultPanel());
    }

    IEnumerator AnimateResultPanel()
    {
        resultPanel.SetActive(true);

        RectTransform resultRect = resultPanel.GetComponent<RectTransform>();

        CanvasGroup cg = resultPanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = resultPanel.AddComponent<CanvasGroup>();

        Vector2 startPos = resultRect.anchoredPosition - new Vector2(0, resultSlideOffset);
        Vector2 targetPos = resultRect.anchoredPosition;

        cg.alpha = 0f;
        resultRect.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * resultAnimationSpeed;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed);
            resultRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        cg.alpha = 1f;
        resultRect.anchoredPosition = targetPos;
    }

    public void CloseCardGame()
    {
        ResetCardGame();
        cardGameCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void ResetCardGame()
    {
        foreach (Card card in playerHand)
            if (card != null)
                Destroy(card.gameObject);
        playerHand.Clear();

        DelayBarManager.Instance.ResetDelayBar();

        playerDeck = new DeckManager();
        enemyDeck = new DeckManager();

        selectedForDiscard.Clear();
        isDiscardMode = false;
        isPlayerTurn = true;
        hasDiscardedThisTurn = false;
        playerLife = 0;
        playerActionPoints = 0;
        playerDefense = 0;
        enemyLife = 0;
        enemyActionPoints = 0;
        enemyDefense = 0;

        endTurnButton.interactable = true;
        discardButton.interactable = true;
        discardButton.GetComponentInChildren<Text>().text = "Défausser";
    }

    void UpdateUI()
    {
        if (playerLifeText != null) playerLifeText.text = "PV : " + playerLife;
        if (playerActionText != null) playerActionText.text = "PA : " + playerActionPoints;
        if (playerDefenseText != null) playerDefenseText.text = "DEF : " + playerDefense;
        if (enemyLifeText != null) enemyLifeText.text = "PV : " + enemyLife;
        if (enemyActionText != null) enemyActionText.text = "PA : " + enemyActionPoints;
        if (enemyDefenseText != null) enemyDefenseText.text = "DEF : " + enemyDefense;

        foreach (Card card in playerHand)
        {
            bool canPlay = false;
            switch (card.cardData.costType)
            {
                case CardData.CostType.ActionPoints:
                    canPlay = playerActionPoints >= card.cardData.actionCost;
                    break;
                case CardData.CostType.Life:
                    canPlay = playerLife > card.cardData.actionCost;
                    break;
                case CardData.CostType.Defense:
                    canPlay = playerDefense >= card.cardData.actionCost;
                    break;
            }
            card.SetPlayable(isPlayerTurn && canPlay);
        }
    }

    public void ApplyDelayedCard(CardData card, bool isPlayer)
    {
        ApplyCardEffect(card, isPlayer);
        UpdateUI();
        CheckGameOver();
    }
}