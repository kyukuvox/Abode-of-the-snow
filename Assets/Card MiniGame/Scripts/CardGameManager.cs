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

    [Header("Main Ennemie")]
    public Transform enemyHandZone;
    public GameObject enemyCardBackPrefab;
    public int enemyHandSize = 3;

    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("Résultat")]
    public GameObject resultPanel;
    public Text resultText;
    public float resultAnimationSpeed = 5f;
    public float resultSlideOffset = 50f;

    [Header("Boutons")]
    public Button endTurnButton;
    public Button discardButton;

    [Header("Boutons Sprites")]
    public Image endTurnButtonImage;
    public Image discardButtonImage;
    public Sprite discardNormalSprite;
    public Sprite discardUsedSprite;
    public Sprite discardConfirmSprite;
    public Sprite endTurnNormalSprite;
    public Sprite endTurnDisabledSprite;

    [Header("Deck Builder")]
    public DeckBuilderManager deckBuilderManager;

    [Header("Shake")]
    public float shakeIntensity = 5f;
    public float shakeDuration = 0.3f;

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
    private bool isGameEnded = false;

    private Item[] rewardItems;
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

    public void StartCardGame(CharacterCardData enemy, CharacterCardData player, Item[] rewards, NPCWithItemDialogue npc)
    {
        currentNPC = npc;
        enemyData = enemy;
        playerData = player;
        rewardItems = rewards;

        playerLife = player.maxLife;
        playerActionPoints = player.actionPointsPerTurn;
        playerDefense = 0;

        enemyLife = enemy.maxLife;
        enemyActionPoints = enemy.actionPointsPerTurn;
        enemyDefense = 0;

        List<CardData> playerDeckCards = deckBuilderManager != null ?
            deckBuilderManager.GetCurrentDeck() : new List<CardData>();

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

        InitEnemyHand();

        UpdateUI();
        isPlayerTurn = true;
        isGameEnded = false;
    }

    void InitEnemyHand()
    {
        foreach (Transform child in enemyHandZone)
            Destroy(child.gameObject);

        for (int i = 0; i < enemyHandSize; i++)
        {
            GameObject card = Instantiate(enemyCardBackPrefab, enemyHandZone);
            StartCoroutine(AnimateEnemyCardDraw(card));
        }
    }

    IEnumerator AnimateEnemyCardDraw(GameObject card)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        CanvasGroup cg = card.GetComponent<CanvasGroup>();
        if (cg == null) cg = card.AddComponent<CanvasGroup>();

        Vector2 startPos = rect.anchoredPosition + new Vector2(0, 50f);
        Vector2 targetPos = rect.anchoredPosition;

        cg.alpha = 0f;
        rect.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 5f;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        cg.alpha = 1f;
        rect.anchoredPosition = targetPos;
    }

    IEnumerator AnimateEnemyCardPlay(int cardIndex)
    {
        if (enemyHandZone.childCount <= cardIndex) yield break;

        Transform card = enemyHandZone.GetChild(cardIndex);
        RectTransform rect = card.GetComponent<RectTransform>();
        CanvasGroup cg = card.GetComponent<CanvasGroup>();
        if (cg == null) cg = card.gameObject.AddComponent<CanvasGroup>();

        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0, -100f);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 5f;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        Destroy(card.gameObject);
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
            if (discardButtonImage != null && discardConfirmSprite != null)
                discardButtonImage.sprite = discardConfirmSprite;
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
                if (discardButtonImage != null && discardNormalSprite != null)
                    discardButtonImage.sprite = discardNormalSprite;
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

        if (discardButtonImage != null && discardUsedSprite != null)
            discardButtonImage.sprite = discardUsedSprite;

        UpdateUI();
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;
        isPlayerTurn = false;
        endTurnButton.interactable = false;

        if (endTurnButtonImage != null && endTurnDisabledSprite != null)
            endTurnButtonImage.sprite = endTurnDisabledSprite;

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

                StartCoroutine(AnimateEnemyCardPlay(0));
                yield return new WaitForSeconds(0.3f);

                if (card.delayTurns > 0)
                    DelayBarManager.Instance.AddDelayedCard(card, card.delayTurns, false);
                else
                    ApplyCardEffect(card, false);

                enemyDeck.DiscardCard(card);
                UpdateUI();

                if (CheckGameOver()) yield break;
            }
            else
            {
                enemyDeck.DiscardCard(card);
            }
        }

        yield return new WaitForSeconds(0.5f);

        InitEnemyHand();

        isPlayerTurn = true;
        endTurnButton.interactable = true;
        playerActionPoints = playerData.actionPointsPerTurn;
        hasDiscardedThisTurn = false;
        discardButton.interactable = true;

        if (discardButtonImage != null && discardNormalSprite != null)
            discardButtonImage.sprite = discardNormalSprite;
        if (endTurnButtonImage != null && endTurnNormalSprite != null)
            endTurnButtonImage.sprite = endTurnNormalSprite;

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
        if (isGameEnded) return false;

        if (enemyLife <= 0)
        {
            isGameEnded = true;
            EndGame(true);
            return true;
        }
        if (playerLife <= 0)
        {
            isGameEnded = true;
            EndGame(false);
            return true;
        }
        return false;
    }

    void EndGame(bool playerWon)
    {
        resultPanel.SetActive(true);
        resultText.text = playerWon ? "Victoire !" : "Défaite...";

        if (currentNPC != null)
            currentNPC.SetDefeated(playerWon);

        if (playerWon)
        {
            if (rewardItems != null && rewardItems.Length > 0)
                StartCoroutine(ShowRewardsSequentially());
            else
                StartCoroutine(ShowDefeatedDialogueAfterDelay());

            if (enemyData.rewardCard != null)
                PlayerCardCollection.Instance.AddCard(enemyData.rewardCard);
        }

        StartCoroutine(AnimateResultPanel());
    }

    IEnumerator ShowRewardsSequentially()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Item item in rewardItems)
        {
            if (item == null) continue;

            Inventory.Instance.AddItem(item);

            yield return new WaitUntil(() => !DialogueManager.Instance.IsActive());

            ItemDescriptionManager.Instance.ShowItemDescription(item);
            yield return new WaitUntil(() => !ItemDescriptionManager.Instance.IsActive());
            yield return new WaitForSeconds(0.3f);
        }

        StartCoroutine(ShowDefeatedDialogueAfterDelay());
    }

    IEnumerator ShowDefeatedDialogueAfterDelay()
    {
        yield return new WaitUntil(() => !ItemDescriptionManager.Instance.IsActive());
        yield return new WaitForSeconds(0.5f);

        if (currentNPC != null && currentNPC.HasBeenDefeated())
        {
            NPCWithItemDialogue npcDialogue = currentNPC.GetComponent<NPCWithItemDialogue>();
            if (npcDialogue != null && npcDialogue.alreadyDefeatedDialogue != null)
            {
                resultPanel.SetActive(false);
                cardGameCanvas.SetActive(false);
                DialogueManager.Instance.StartDialogue(npcDialogue.alreadyDefeatedDialogue, currentNPC);
            }
        }
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

        if (enemyHandZone != null)
            foreach (Transform child in enemyHandZone)
                Destroy(child.gameObject);

        DelayBarManager.Instance.ResetDelayBar();

        playerDeck = new DeckManager();
        enemyDeck = new DeckManager();

        selectedForDiscard.Clear();
        isDiscardMode = false;
        isPlayerTurn = true;
        hasDiscardedThisTurn = false;
        isGameEnded = false;
        playerLife = 0;
        playerActionPoints = 0;
        playerDefense = 0;
        enemyLife = 0;
        enemyActionPoints = 0;
        enemyDefense = 0;

        endTurnButton.interactable = true;
        discardButton.interactable = true;

        if (discardButtonImage != null && discardNormalSprite != null)
            discardButtonImage.sprite = discardNormalSprite;
        if (endTurnButtonImage != null && endTurnNormalSprite != null)
            endTurnButtonImage.sprite = endTurnNormalSprite;
    }

    void UpdateUI()
    {
        if (playerLifeText != null) playerLifeText.text = "" + playerLife;
        if (playerActionText != null) playerActionText.text = "" + playerActionPoints;
        if (playerDefenseText != null) playerDefenseText.text = "" + playerDefense;
        if (enemyLifeText != null) enemyLifeText.text = "" + enemyLife;
        if (enemyActionText != null) enemyActionText.text = "" + enemyActionPoints;
        if (enemyDefenseText != null) enemyDefenseText.text = "" + enemyDefense;

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

    public void SwapCards(Card cardA, Card cardB)
    {
        int indexA = playerHand.IndexOf(cardA);
        int indexB = playerHand.IndexOf(cardB);

        if (indexA == -1 || indexB == -1) return;

        playerHand[indexA] = cardB;
        playerHand[indexB] = cardA;
    }

    public void ApplyDelayedCard(CardData card, bool isPlayer)
    {
        ApplyCardEffect(card, isPlayer);
        UpdateUI();
        CheckGameOver();
    }
}