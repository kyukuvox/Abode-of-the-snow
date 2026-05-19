using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance;
    public static bool IsPlaying { get; private set; } = false;

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

    [Header("Prefabs")]
    public GameObject cardPrefab;

    [Header("Résultat")]
    public GameObject resultPanel;
    public Text resultText;

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

    [Header("Intro")]
    public float enemySpriteStartOffset = 300f;
    public float enemySpriteDuration = 0.8f;

    [Header("Fade")]
    public float resultFadeDuration = 0.5f;
    public Image fadePanel;

    [Header("Récompense Carte")]
    public GameObject cardRewardPanel;
    public Text cardRewardText;
    public float cardRewardDisplayDuration = 3f;
    public float cardRewardFadeDuration = 0.5f;

    [Header("Tutoriel")]
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public Button tutorialLeftButton;
    public Button tutorialRightButton;
    public Sprite[] tutorialSprites;
    public float tutorialFadeDuration = 0.3f;
    public AudioClip tutorialButtonSound;
    [Range(0f, 1f)]
    public float tutorialButtonVolume = 1f;

    [Header("Sons")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    public float musicFadeDuration = 1f;
    public AudioClip playerCardDrawSound;
    [Range(0f, 1f)]
    public float playerCardDrawVolume = 1f;
    public AudioClip enemyCardDrawSound;
    [Range(0f, 1f)]
    public float enemyCardDrawVolume = 1f;
    public AudioClip characterIntroSound;
    [Range(0f, 1f)]
    public float characterIntroVolume = 1f;
    public AudioClip playCardSound;
    [Range(0f, 1f)]
    public float playCardVolume = 1f;
    public AudioClip playerDamageSound;
    [Range(0f, 1f)]
    public float playerDamageVolume = 1f;
    public AudioClip enemyDamageSound;
    [Range(0f, 1f)]
    public float enemyDamageVolume = 1f;
    public AudioClip statBoostSound;
    [Range(0f, 1f)]
    public float statBoostVolume = 1f;

    [Header("Sons boutons")]
    public AudioClip endTurnButtonSound;
    [Range(0f, 1f)]
    public float endTurnButtonVolume = 1f;
    public AudioClip discardButtonSound;
    [Range(0f, 1f)]
    public float discardButtonVolume = 1f;
    public AudioClip closeResultButtonSound;
    [Range(0f, 1f)]
    public float closeResultButtonVolume = 1f;

    [Header("Musiques résultat")]
    public AudioClip victoryMusic;
    [Range(0f, 1f)]
    public float victoryMusicVolume = 1f;
    public AudioClip defeatMusic;
    [Range(0f, 1f)]
    public float defeatMusicVolume = 1f;

    private AudioSource musicAudioSource;
    private AudioSource resultMusicSource;

    private int tutorialIndex = 0;
    private const string TUTORIAL_KEY = "CardGameTutorialShown";

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
    private bool rewardsDone = false;
    private bool activatesPortalAfterGame = false;
    private bool playerWonLastGame = false;

    private Item[] rewardItems;
    private CardData rewardCard;
    private NPCWithItemDialogue currentNPC;

    private List<Vector2> enemyCardPositions = new List<Vector2>();

    private Vector2 playerLifeInitPos;
    private Vector2 playerActionInitPos;
    private Vector2 playerDefenseInitPos;
    private Vector2 enemyLifeInitPos;
    private Vector2 enemyActionInitPos;
    private Vector2 enemyDefenseInitPos;

    void Awake()
    {
        Instance = this;

        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.spatialBlend = 0f;
        musicAudioSource.volume = 0f;

        resultMusicSource = gameObject.AddComponent<AudioSource>();
        resultMusicSource.loop = false;
        resultMusicSource.playOnAwake = false;
        resultMusicSource.spatialBlend = 0f;
        resultMusicSource.volume = 0f;
    }

    void Start()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        endTurnButton.onClick.AddListener(() => SoundSettings.PlaySound(endTurnButtonSound, endTurnButtonVolume, this));

        discardButton.onClick.AddListener(ToggleDiscardMode);
        discardButton.onClick.AddListener(() => SoundSettings.PlaySound(discardButtonSound, discardButtonVolume, this));

        Button closeButton = resultPanel.GetComponentInChildren<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCardGame);
            closeButton.onClick.AddListener(() => SoundSettings.PlaySound(closeResultButtonSound, closeResultButtonVolume, this));
        }

        if (cardRewardPanel != null)
            cardRewardPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (tutorialLeftButton != null)
            tutorialLeftButton.onClick.AddListener(TutorialPrevious);
        if (tutorialRightButton != null)
            tutorialRightButton.onClick.AddListener(TutorialNext);

        playerLifeInitPos = playerLifeText.rectTransform.anchoredPosition;
        playerActionInitPos = playerActionText.rectTransform.anchoredPosition;
        playerDefenseInitPos = playerDefenseText.rectTransform.anchoredPosition;
        enemyLifeInitPos = enemyLifeText.rectTransform.anchoredPosition;
        enemyActionInitPos = enemyActionText.rectTransform.anchoredPosition;
        enemyDefenseInitPos = enemyDefenseText.rectTransform.anchoredPosition;
    }

    void ResetTextPositions()
    {
        playerLifeText.rectTransform.anchoredPosition = playerLifeInitPos;
        playerActionText.rectTransform.anchoredPosition = playerActionInitPos;
        playerDefenseText.rectTransform.anchoredPosition = playerDefenseInitPos;
        enemyLifeText.rectTransform.anchoredPosition = enemyLifeInitPos;
        enemyActionText.rectTransform.anchoredPosition = enemyActionInitPos;
        enemyDefenseText.rectTransform.anchoredPosition = enemyDefenseInitPos;
    }

    void PlaySound(AudioClip clip, float volume)
    {
        SoundSettings.PlaySound(clip, volume, this);
    }

    IEnumerator FadeInMusic()
    {
        if (backgroundMusic == null) yield break;
        musicAudioSource.clip = backgroundMusic;
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();

        float elapsed = 0f;
        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, musicVolume * SoundSettings.MusicVolume, elapsed / musicFadeDuration);
            yield return null;
        }
        musicAudioSource.volume = musicVolume * SoundSettings.MusicVolume;
    }

    IEnumerator FadeOutMusic()
    {
        float startVolume = musicAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
            yield return null;
        }

        musicAudioSource.volume = 0f;
        musicAudioSource.Stop();
    }

    IEnumerator FadeInResultMusic(float targetVolume)
    {
        float elapsed = 0f;
        while (elapsed < resultFadeDuration)
        {
            elapsed += Time.deltaTime;
            resultMusicSource.volume = Mathf.Lerp(0f, targetVolume * SoundSettings.MusicVolume, elapsed / resultFadeDuration);
            yield return null;
        }
        resultMusicSource.volume = targetVolume * SoundSettings.MusicVolume;
    }

    IEnumerator FadeOutResultMusic()
    {
        float startVolume = resultMusicSource.volume;
        float elapsed = 0f;

        while (elapsed < resultFadeDuration)
        {
            elapsed += Time.deltaTime;
            resultMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / resultFadeDuration);
            yield return null;
        }

        resultMusicSource.volume = 0f;
        resultMusicSource.Stop();
    }

    IEnumerator ResumeAmbientMusic()
    {
        yield return new WaitForSeconds(musicFadeDuration);
        if (MusicManager.Instance != null)
            MusicManager.Instance.ResumeMusic(musicFadeDuration);
    }

    public void ApplyMusicVolume(float volume)
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
            musicAudioSource.volume = volume * musicVolume;
        if (resultMusicSource != null && resultMusicSource.isPlaying)
            resultMusicSource.volume = volume * (playerWonLastGame ? victoryMusicVolume : defeatMusicVolume);
    }

    public void StartCardGame(CharacterCardData enemy, CharacterCardData player, Item[] rewards, CardData cardReward, bool activatesPortal, NPCWithItemDialogue npc)
    {
        currentNPC = npc;
        enemyData = enemy;
        playerData = player;
        rewardItems = rewards;
        rewardCard = cardReward;
        activatesPortalAfterGame = activatesPortal;
        rewardsDone = false;
        playerWonLastGame = false;

        if (currentNPC != null)
            currentNPC.SetFought();

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

        enemyCardSprite.sprite = enemy.characterSprite;

        cardGameCanvas.SetActive(true);
        resultPanel.SetActive(false);

        if (cardRewardPanel != null)
            cardRewardPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }

        isPlayerTurn = true;
        isGameEnded = false;
        IsPlaying = true;

        ResetTextPositions();

        StartCoroutine(CardGameIntro());
    }

    IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;

        float elapsed = 0f;
        while (elapsed < resultFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / resultFadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 1f;
        fadePanel.color = c;
    }

    IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;
        c.a = 1f;
        fadePanel.color = c;

        float elapsed = 0f;
        while (elapsed < resultFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / resultFadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }

    IEnumerator CardGameIntro()
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;
        c.a = 1f;
        fadePanel.color = c;

        RectTransform enemySpriteRect = enemyCardSprite.GetComponent<RectTransform>();
        Vector2 targetPos = enemySpriteRect.anchoredPosition;
        Vector2 startPos = targetPos + new Vector2(0, enemySpriteStartOffset);
        enemySpriteRect.anchoredPosition = startPos;

        yield return StartCoroutine(FadeOut());

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic(musicFadeDuration);

        PlaySound(characterIntroSound, characterIntroVolume);
        StartCoroutine(FadeInMusic());

        float elapsed = 0f;
        while (elapsed < enemySpriteDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / enemySpriteDuration;
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            enemySpriteRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        enemySpriteRect.anchoredPosition = targetPos;

        yield return new WaitForSeconds(0.2f);

        if (!PlayerPrefs.HasKey(TUTORIAL_KEY))
            yield return StartCoroutine(ShowTutorial());

        int safetyLimit = 0;
        while (playerHand.Count < 5 && safetyLimit < 20)
        {
            DrawPlayerCard();
            yield return new WaitForSeconds(0.1f);
            safetyLimit++;
        }

        InitEnemyHand();
        UpdateUI();
    }

    IEnumerator ShowTutorial()
    {
        if (tutorialPanel == null || tutorialSprites == null || tutorialSprites.Length == 0)
            yield break;

        tutorialIndex = 0;
        UpdateTutorialImage();

        CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = tutorialPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        tutorialPanel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < tutorialFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / tutorialFadeDuration);
            yield return null;
        }
        cg.alpha = 1f;

        yield return new WaitUntil(() => !tutorialPanel.activeSelf);

        PlayerPrefs.SetInt(TUTORIAL_KEY, 1);
        PlayerPrefs.Save();
    }

    void UpdateTutorialImage()
    {
        if (tutorialImage != null && tutorialSprites != null && tutorialIndex < tutorialSprites.Length)
            tutorialImage.sprite = tutorialSprites[tutorialIndex];

        if (tutorialLeftButton != null)
            tutorialLeftButton.gameObject.SetActive(tutorialIndex > 0);
    }

    void TutorialPrevious()
    {
        if (tutorialIndex > 0)
        {
            SoundSettings.PlaySound(tutorialButtonSound, tutorialButtonVolume, this);
            tutorialIndex--;
            UpdateTutorialImage();
        }
    }

    void TutorialNext()
    {
        if (tutorialSprites == null) return;

        SoundSettings.PlaySound(tutorialButtonSound, tutorialButtonVolume, this);
        tutorialIndex++;

        if (tutorialIndex >= tutorialSprites.Length)
        {
            StartCoroutine(CloseTutorialWithFade());
            return;
        }

        UpdateTutorialImage();
    }

    IEnumerator CloseTutorialWithFade()
    {
        CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = tutorialPanel.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < tutorialFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / tutorialFadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
        tutorialPanel.SetActive(false);
    }

    void InitEnemyHand()
    {
        foreach (Transform child in enemyHandZone)
            Destroy(child.gameObject);

        StartCoroutine(SpawnEnemyCards());
    }

    IEnumerator SpawnEnemyCards()
    {
        yield return null;

        float cardWidth = 60f;
        float cardHeight = 90f;
        float spacing = 5f;

        int handSize = enemyData.handSize;

        float totalWidth = (handSize * cardWidth) + ((handSize - 1) * spacing);
        float startX = -totalWidth / 2f + cardWidth / 2f;

        enemyCardPositions.Clear();

        for (int i = 0; i < handSize; i++)
        {
            GameObject card = Instantiate(enemyCardBackPrefab, enemyHandZone);

            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(cardWidth, cardHeight);

            float xPos = startX + i * (cardWidth + spacing);
            Vector2 enemyTargetPos = new Vector2(xPos, 0f);
            rect.anchoredPosition = enemyTargetPos;

            enemyCardPositions.Add(enemyTargetPos);

            StartCoroutine(AnimateEnemyCardDraw(card, enemyTargetPos));
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator AnimateEnemyCardDraw(GameObject card, Vector2 targetPos)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        CanvasGroup cg = card.GetComponent<CanvasGroup>();
        if (cg == null) cg = card.AddComponent<CanvasGroup>();

        Vector2 startPos = targetPos + new Vector2(0, 50f);

        cg.alpha = 0f;
        rect.anchoredPosition = startPos;

        PlaySound(enemyCardDrawSound, enemyCardDrawVolume);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 8f;
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

    void DrawPlayerCard()
    {
        CardData data = playerDeck.DrawCard();
        if (data == null) return;

        GameObject cardObj = Instantiate(cardPrefab, playerHandZone);
        Card card = cardObj.GetComponent<Card>();
        card.Setup(data);
        playerHand.Add(card);
        StartCoroutine(AnimatePlayerCardDraw(cardObj));
    }

    IEnumerator AnimatePlayerCardDraw(GameObject cardObj)
    {
        CanvasGroup cg = cardObj.GetComponent<CanvasGroup>();
        if (cg == null) cg = cardObj.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        PlaySound(playerCardDrawSound, playerCardDrawVolume);

        yield return null;
        yield return null;
        yield return null;
        yield return null;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * 4f;
            cg.alpha = Mathf.Clamp01(elapsed);
            yield return null;
        }

        cg.alpha = 1f;

        Card card = cardObj.GetComponent<Card>();
        if (card != null)
            card.InitRestPosition();
    }

    public void PlayCard(Card card)
    {
        if (!isPlayerTurn) return;

        PlaySound(playCardSound, playCardVolume);

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
                    PlaySound(enemyDamageSound, enemyDamageVolume);
                    ApplyAttack(card, ref enemyDefense, ref enemyActionPoints, ref enemyLife, true);
                    break;
                case CardData.CardType.Defense:
                    PlaySound(statBoostSound, statBoostVolume);
                    playerDefense += card.power;
                    break;
                case CardData.CardType.Recharge:
                    PlaySound(statBoostSound, statBoostVolume);
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
                    PlaySound(playerDamageSound, playerDamageVolume);
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
        Vector2 originalPos = text.rectTransform.anchoredPosition;
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

        int attempts = 0;
        for (int i = 0; i < 2; i++)
        {
            if (attempts >= 10) break;
            DrawPlayerCard();
            attempts++;
        }

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

        CardData[] hand = new CardData[enemyData.handSize];
        for (int i = 0; i < enemyData.handSize; i++)
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

        int drawAttempts = 0;
        while (playerHand.Count < 5 && drawAttempts < 20)
        {
            DrawPlayerCard();
            yield return new WaitForSeconds(0.1f);
            drawAttempts++;
        }

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
        StopAllCoroutines();

        StartCoroutine(FadeOutMusic());
        StartCoroutine(ResumeAmbientMusic());

        rewardsDone = false;
        playerWonLastGame = playerWon;

        if (playerWon)
        {
            GameStateManager.Instance.SetCinematicMode(true);
            if (currentNPC != null)
                currentNPC.SetDefeated(true);
        }
        else
        {
            GameStateManager.Instance.SetCinematicMode(false);
            rewardsDone = true;
        }

        StartCoroutine(ShowResultWithFade(playerWon));
    }

    IEnumerator ShowResultWithFade(bool playerWon)
    {
        yield return StartCoroutine(FadeIn());

        resultPanel.SetActive(true);
        resultText.text = playerWon ? "Victoire !" : "Défaite...";

        AudioClip resultClip = playerWon ? victoryMusic : defeatMusic;
        float resultVolume = playerWon ? victoryMusicVolume : defeatMusicVolume;

        if (resultClip != null)
        {
            resultMusicSource.clip = resultClip;
            resultMusicSource.volume = 0f;
            resultMusicSource.Play();
            StartCoroutine(FadeInResultMusic(resultVolume));
        }

        yield return StartCoroutine(FadeOut());

        if (!playerWon) yield break;

        bool hasRewards = rewardItems != null && rewardItems.Length > 0;
        bool hasCardReward = rewardCard != null;

        if (hasRewards || hasCardReward)
            StartCoroutine(ShowRewardsSequentially());
        else
            rewardsDone = true;
    }

    IEnumerator ShowCardReward()
    {
        if (cardRewardPanel == null) yield break;

        if (cardRewardText != null)
            cardRewardText.text = "+1 carte dans la collection";

        CanvasGroup cg = cardRewardPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = cardRewardPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        cardRewardPanel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < cardRewardFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / cardRewardFadeDuration);
            yield return null;
        }
        cg.alpha = 1f;

        yield return new WaitForSeconds(cardRewardDisplayDuration);

        elapsed = 0f;
        while (elapsed < cardRewardFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / cardRewardFadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
        cardRewardPanel.SetActive(false);
    }

    IEnumerator ShowRewardsSequentially()
    {
        if (!playerWonLastGame)
        {
            rewardsDone = true;
            GameStateManager.Instance.SetCinematicMode(false);
            yield break;
        }

        rewardsDone = false;

        yield return new WaitForSeconds(0.5f);

        if (rewardItems != null)
        {
            foreach (Item item in rewardItems)
            {
                if (item == null) continue;
                if (!playerWonLastGame) break;

                Inventory.Instance.AddItem(item);

                yield return new WaitUntil(() => !DialogueManager.Instance.IsActive());

                ItemDescriptionManager.Instance.ShowItemDescription(item);
                yield return new WaitUntil(() => !ItemDescriptionManager.Instance.IsActive());
                yield return new WaitForSeconds(0.3f);
            }
        }

        rewardsDone = true;
    }

    IEnumerator ShowDefeatedDialogueAfterDelay()
    {
        yield return new WaitUntil(() => rewardsDone);
        yield return new WaitForSeconds(0.5f);

        if (currentNPC != null && currentNPC.HasBeenDefeated())
        {
            NPCWithItemDialogue npcDialogue = currentNPC.GetComponent<NPCWithItemDialogue>();
            if (npcDialogue != null && npcDialogue.alreadyDefeatedDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(npcDialogue.alreadyDefeatedDialogue, currentNPC);
                yield return new WaitUntil(() => !DialogueManager.Instance.IsActive());
            }
        }

        if (activatesPortalAfterGame)
        {
            PortalAnimator[] allPortals = FindObjectsByType<PortalAnimator>(FindObjectsSortMode.None);
            foreach (PortalAnimator portal in allPortals)
            {
                if (portal.onlyAfterCardGame)
                    portal.ActivatePortal();
            }

            activatesPortalAfterGame = false;
        }

        GameStateManager.Instance.SetCinematicMode(false);
    }

    public void CloseCardGame()
    {
        StartCoroutine(CloseWithFade());
    }

    IEnumerator CloseWithFade()
    {
        yield return StartCoroutine(FadeOutResultMusic());

        yield return StartCoroutine(HideResultWithFade());
        ResetCardGame();

        if (playerWonLastGame)
        {
            if (rewardCard != null)
            {
                PlayerCardCollection.Instance.AddCard(rewardCard);
                StartCoroutine(ShowCardReward());
            }
            StartCoroutine(ShowDefeatedDialogueAfterDelay());
        }
        else
        {
            if (currentNPC != null)
            {
                Collider2D col = currentNPC.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                NPCWithItemDialogue npcDialogue = currentNPC.GetComponent<NPCWithItemDialogue>();
                if (npcDialogue != null) npcDialogue.enabled = true;
            }

            GameStateManager.Instance.SetCinematicMode(false);
        }

        Time.timeScale = 1f;
    }

    IEnumerator HideResultWithFade()
    {
        yield return StartCoroutine(FadeIn());

        resultPanel.SetActive(false);
        cardGameCanvas.SetActive(false);

        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
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

        IsPlaying = false;

        endTurnButton.interactable = true;
        discardButton.interactable = true;

        if (discardButtonImage != null && discardNormalSprite != null)
            discardButtonImage.sprite = discardNormalSprite;
        if (endTurnButtonImage != null && endTurnNormalSprite != null)
            endTurnButtonImage.sprite = endTurnNormalSprite;

        ResetTextPositions();
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