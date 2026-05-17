using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeckBuilderManager : MonoBehaviour
{
    public static DeckBuilderManager Instance;

    [Header("Slots du deck")]
    public GameObject deckSlotPrefab;
    public Transform deckSlotsZone;
    public int maxDeckSize = 8;

    [Header("Navigateur de cartes")]
    public Image browserCardImage;
    public Text browserCardNameText;
    public Button browserLeftArrow;
    public Button browserRightArrow;
    public Button assignCardButton;

    [Header("Visuel")]
    public Color selectedOutlineColor = new Color(1f, 1f, 0f, 1f);
    public Color flashColor = Color.yellow;
    public float flashDuration = 0.3f;

    [Header("Sons")]
    public AudioClip slotSelectSound;
    [Range(0f, 1f)]
    public float slotSelectVolume = 1f;
    public AudioClip assignButtonSound;
    [Range(0f, 1f)]
    public float assignButtonVolume = 1f;
    public AudioClip cardAssignedSound;
    [Range(0f, 1f)]
    public float cardAssignedVolume = 1f;

    private List<CardData> currentDeck = new List<CardData>();
    private List<GameObject> deckSlots = new List<GameObject>();
    private int selectedSlotIndex = -1;
    private int browserIndex = 0;
    private bool isInitialized = false;

    void Awake()
    {
        Instance = this;
        currentDeck = new List<CardData>(new CardData[maxDeckSize]);
    }

    void OnEnable()
    {
        if (browserLeftArrow != null)
        {
            Navigation nav = browserLeftArrow.navigation;
            nav.mode = Navigation.Mode.None;
            browserLeftArrow.navigation = nav;
        }
        if (browserRightArrow != null)
        {
            Navigation nav = browserRightArrow.navigation;
            nav.mode = Navigation.Mode.None;
            browserRightArrow.navigation = nav;
        }
        if (assignCardButton != null)
        {
            Navigation nav = assignCardButton.navigation;
            nav.mode = Navigation.Mode.None;
            assignCardButton.navigation = nav;
        }

        if (!isInitialized)
        {
            InitializeSlots();
            isInitialized = true;
        }

        browserLeftArrow.onClick.RemoveAllListeners();
        browserRightArrow.onClick.RemoveAllListeners();
        assignCardButton.onClick.RemoveAllListeners();
        browserLeftArrow.onClick.AddListener(PreviousBrowserCard);
        browserRightArrow.onClick.AddListener(NextBrowserCard);
        assignCardButton.onClick.AddListener(AssignCardToSlot);

        browserIndex = 0;
        selectedSlotIndex = -1;

        UpdateSlotsDisplay();
        StartCoroutine(InitBrowserDelayed());
    }

    IEnumerator InitBrowserDelayed()
    {
        yield return new WaitUntil(() => PlayerCardCollection.Instance != null);
        yield return null;
        yield return null;

        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        if (cards.Count > 0)
            DisplayBrowserCard(0);
    }

    void InitializeSlots()
    {
        foreach (Transform child in deckSlotsZone)
            Destroy(child.gameObject);
        deckSlots.Clear();

        for (int i = 0; i < maxDeckSize; i++)
        {
            int index = i;
            GameObject slot = Instantiate(deckSlotPrefab, deckSlotsZone);
            Button btn = slot.GetComponent<Button>();

            if (btn == null)
                btn = slot.AddComponent<Button>();

            Navigation nav = btn.navigation;
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            Outline outline = slot.GetComponent<Outline>();
            if (outline == null)
                outline = slot.AddComponent<Outline>();
            outline.enabled = false;
            outline.effectColor = selectedOutlineColor;
            outline.effectDistance = new Vector2(3f, 3f);

            btn.onClick.AddListener(() => SelectSlot(index));
            deckSlots.Add(slot);
        }
    }

    void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        SoundSettings.PlaySound(slotSelectSound, slotSelectVolume, this);
        UpdateSlotsDisplay();
    }

    void UpdateSlotsDisplay()
    {
        for (int i = 0; i < deckSlots.Count; i++)
        {
            Image slotBg = deckSlots[i].GetComponent<Image>();
            Transform slotCardTransform = deckSlots[i].transform.Find("SlotCardImage");
            Outline outline = deckSlots[i].GetComponent<Outline>();

            if (outline != null)
                outline.enabled = (i == selectedSlotIndex);

            if (slotCardTransform == null) continue;

            Image cardImg = slotCardTransform.GetComponent<Image>();

            if (currentDeck[i] != null)
            {
                cardImg.gameObject.SetActive(true);
                cardImg.sprite = currentDeck[i].cardSprite;
                slotBg.color = Color.white;
            }
            else
            {
                cardImg.gameObject.SetActive(false);
                slotBg.color = i == selectedSlotIndex ?
                    new Color(0.5f, 0.5f, 1f, 1f) :
                    new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }
    }

    public void PreviousBrowserCard()
    {
        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        if (cards.Count == 0) return;
        browserIndex--;
        if (browserIndex < 0)
            browserIndex = cards.Count - 1;
        DisplayBrowserCard(browserIndex);
    }

    public void NextBrowserCard()
    {
        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        if (cards.Count == 0) return;
        browserIndex++;
        if (browserIndex >= cards.Count)
            browserIndex = 0;
        DisplayBrowserCard(browserIndex);
    }

    void DisplayBrowserCard(int index)
    {
        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();

        if (cards.Count == 0 || index >= cards.Count) return;

        CardData card = cards[index];

        if (browserCardImage != null)
        {
            browserCardImage.enabled = true;
            browserCardImage.gameObject.SetActive(true);
            browserCardImage.sprite = card.cardSprite;
            browserCardImage.color = Color.white;
        }

        if (browserCardNameText != null)
            browserCardNameText.text = card.cardName;
    }

    void AssignCardToSlot()
    {
        if (selectedSlotIndex == -1) return;

        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        if (cards.Count == 0) return;

        SoundSettings.PlaySound(assignButtonSound, assignButtonVolume, this);

        int assignedIndex = selectedSlotIndex;
        currentDeck[assignedIndex] = cards[browserIndex];

        selectedSlotIndex = -1;
        UpdateSlotsDisplay();

        StartCoroutine(FlashSlot(deckSlots[assignedIndex]));

        SoundSettings.PlaySound(cardAssignedSound, cardAssignedVolume, this);
    }

    IEnumerator FlashSlot(GameObject slot)
    {
        yield return null;

        Image slotImage = slot.GetComponent<Image>();
        if (slotImage == null) yield break;

        Color originalColor = slotImage.color;
        float elapsed = 0f;
        float halfDuration = flashDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            slotImage.color = Color.Lerp(originalColor, flashColor, elapsed / halfDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            slotImage.color = Color.Lerp(flashColor, originalColor, elapsed / halfDuration);
            yield return null;
        }

        slotImage.color = originalColor;
    }

    public void LoadDeck(List<CardData> deck)
    {
        for (int i = 0; i < maxDeckSize; i++)
            currentDeck[i] = i < deck.Count ? deck[i] : null;

        if (isInitialized)
            UpdateSlotsDisplay();
    }

    public List<CardData> GetCurrentDeck()
    {
        List<CardData> deck = new List<CardData>();
        foreach (CardData card in currentDeck)
            if (card != null) deck.Add(card);
        return deck;
    }
}