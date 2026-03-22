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
    public Transform playerHandZone;
    public Transform playerDefenseZone;

    [Header("UI Ennemi")]
    public Text enemyLifeText;
    public Text enemyActionText;
    public Transform enemyDefenseZone;
    public Image enemyCardSprite;

    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("Résultat")]
    public GameObject resultPanel;
    public Text resultText;

    [Header("Boutons")]
    public Button endTurnButton;
    public Button discardButton;

    // État du jeu
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
    private List<CardData> selectedForDiscard = new List<CardData>();

    private bool isDiscardMode = false;
    private bool isPlayerTurn = true;

    // Item à donner si victoire
    private Item rewardItem;

    void Awake()
    {
        Instance = this;
    }

    // Appelé par le DialogueManager pour lancer le mini jeu
    public void StartCardGame(CharacterCardData enemy, CharacterCardData player, Item reward)
    {
        Debug.Log("StartCardGame lancé !");
        Debug.Log("Enemy : " + (enemy != null ? enemy.characterName : "NULL"));
        Debug.Log("Player : " + (player != null ? player.characterName : "NULL"));
        Debug.Log("PlayerHandZone : " + (playerHandZone != null ? "OK" : "NULL"));
        Debug.Log("CardPrefab : " + (cardPrefab != null ? "OK" : "NULL"));
        Debug.Log("PlayerLifeText : " + (playerLifeText != null ? "OK" : "NULL"));

        enemyData = enemy;
        playerData = player;
        rewardItem = reward;

        playerLife = player.maxLife;
        playerActionPoints = player.actionPointsPerTurn;
        playerDefense = 0;

        enemyLife = enemy.maxLife;
        enemyActionPoints = enemy.actionPointsPerTurn;
        enemyDefense = 0;

        playerDeck.InitializeDeck(player.startingDeck);
        enemyDeck.InitializeDeck(enemy.startingDeck);

        cardGameCanvas.SetActive(true);
        resultPanel.SetActive(false);

        enemyCardSprite.sprite = enemy.characterSprite;

        for (int i = 0; i < 5; i++)
            DrawPlayerCard();

        Debug.Log("Cartes dans la main : " + playerHand.Count);

        UpdateUI();
        isPlayerTurn = true;
    }

    public void PlayCard(Card card)
    {
        if (!isPlayerTurn) return;
        if (playerActionPoints < card.cardData.actionCost) return;

        playerActionPoints -= card.cardData.actionCost;

        if (card.cardData.delayTurns > 0)
        {
            // Carte délayée → ajoute à la barre de délais
            DelayBarManager.Instance.AddDelayedCard(card.cardData, card.cardData.delayTurns, true);
        }
        else
        {
            ApplyCardEffect(card.cardData, true);
        }

        // Retire la carte de la main
        playerHand.Remove(card);
        playerDeck.DiscardCard(card.cardData);
        Destroy(card.gameObject);

        UpdateUI();
        CheckGameOver();
    }

    void ApplyCardEffect(CardData card, bool isPlayer)
    {
        if (isPlayer)
        {
            switch (card.cardType)
            {
                case CardData.CardType.Attack:
                    ApplyAttack(card, ref enemyDefense, ref enemyActionPoints, ref enemyLife);
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
                    ApplyAttack(card, ref playerDefense, ref playerActionPoints, ref playerLife);
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

    void ApplyAttack(CardData card, ref int defense, ref int actionPoints, ref int life)
    {
        switch (card.attackType)
        {
            case CardData.AttackType.HitDefense:
                defense = Mathf.Max(0, defense - card.power);
                break;
            case CardData.AttackType.HitRecharge:
                actionPoints = Mathf.Max(0, actionPoints - card.power);
                break;
            case CardData.AttackType.HitLife:
                // La défense absorbe d'abord
                int remaining = card.power - defense;
                defense = Mathf.Max(0, defense - card.power);
                if (remaining > 0) life -= remaining;
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

    // --- DÉFAUSSE ---
    public void ToggleDiscardMode()
    {
        isDiscardMode = !isDiscardMode;
        selectedForDiscard.Clear();
        discardButton.GetComponentInChildren<Text>().text =
            isDiscardMode ? "Confirmer défausse" : "Défausser";
    }

    public void SelectCardForDiscard(Card card)
    {
        if (!isDiscardMode) return;

        if (selectedForDiscard.Contains(card.cardData))
        {
            selectedForDiscard.Remove(card.cardData);
        }
        else if (selectedForDiscard.Count < 3)
        {
            selectedForDiscard.Add(card.cardData);
        }

        // Si 3 cartes sélectionnées → défausse automatique
        if (selectedForDiscard.Count == 3)
            ConfirmDiscard();
    }

    void ConfirmDiscard()
    {
        foreach (CardData data in selectedForDiscard)
        {
            playerDeck.DiscardCard(data);
            Card toRemove = playerHand.Find(c => c.cardData == data);
            if (toRemove != null)
            {
                playerHand.Remove(toRemove);
                Destroy(toRemove.gameObject);
            }
        }

        selectedForDiscard.Clear();
        for (int i = 0; i < 3; i++)
            DrawPlayerCard();

        isDiscardMode = false;
        discardButton.GetComponentInChildren<Text>().text = "Défausser";
        UpdateUI();
    }

    // --- FIN DE TOUR ---
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

        // Résout les cartes délayées de l'ennemi
        DelayBarManager.Instance.TickTurn(false);

        // L'ennemi joue des cartes aléatoirement
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

        // Tour du joueur
        isPlayerTurn = true;
        endTurnButton.interactable = true;
        playerActionPoints = playerData.actionPointsPerTurn;

        // Résout les cartes délayées du joueur
        DelayBarManager.Instance.TickTurn(true);

        // Pioche jusqu'à 5 cartes
        while (playerHand.Count < 5)
            DrawPlayerCard();

        // Mise à jour jouabilité des cartes
        foreach (Card card in playerHand)
            card.SetPlayable(playerActionPoints >= card.cardData.actionCost);

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
        resultPanel.SetActive(true);
        resultText.text = playerWon ? "Victoire !" : "Défaite...";

        if (playerWon && rewardItem != null)
            Inventory.Instance.AddItem(rewardItem);
    }

    public void CloseCardGame()
    {
        cardGameCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void UpdateUI()
    {
        playerLifeText.text = "PV : " + playerLife;
        playerActionText.text = "PA : " + playerActionPoints;
        enemyLifeText.text = "PV : " + enemyLife;
        enemyActionText.text = "PA : " + enemyActionPoints;

        foreach (Card card in playerHand)
            card.SetPlayable(isPlayerTurn && playerActionPoints >= card.cardData.actionCost);
    }

    public void ApplyDelayedCard(CardData card, bool isPlayer)
    {
        ApplyCardEffect(card, isPlayer);
        UpdateUI();
        CheckGameOver();
    }
}