using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckBuilderManager : MonoBehaviour
{
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

    private List<CardData> currentDeck = new List<CardData>();
    private List<GameObject> deckSlots = new List<GameObject>();
    private int selectedSlotIndex = -1;
    private int browserIndex = 0;
    private bool isInitialized = false;

    public static DeckBuilderManager Instance;

    void Awake()
    {
        Instance = this;
        currentDeck = new List<CardData>(new CardData[maxDeckSize]);
    }

    public void LoadDeck(List<CardData> deck)
    {
        for (int i = 0; i < maxDeckSize; i++)
            currentDeck[i] = i < deck.Count ? deck[i] : null;

        if (isInitialized)
            UpdateSlotsDisplay();
    }
    void OnEnable()
    {
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

        if (PlayerCardCollection.Instance.GetUnlockedCards().Count > 0)
            DisplayBrowserCard(browserIndex);

        UpdateSlotsDisplay();
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

            btn.onClick.AddListener(() => SelectSlot(index));
            deckSlots.Add(slot);
        }
    }

    void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        UpdateSlotsDisplay();
    }

    void UpdateSlotsDisplay()
    {
        for (int i = 0; i < deckSlots.Count; i++)
        {
            Image slotBg = deckSlots[i].GetComponent<Image>();
            Transform slotCardTransform = deckSlots[i].transform.Find("SlotCardImage");

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

    void PreviousBrowserCard()
    {
        browserIndex--;
        if (browserIndex < 0)
            browserIndex = PlayerCardCollection.Instance.GetUnlockedCards().Count - 1;
        DisplayBrowserCard(browserIndex);
    }

    void NextBrowserCard()
    {
        browserIndex++;
        if (browserIndex >= PlayerCardCollection.Instance.GetUnlockedCards().Count)
            browserIndex = 0;
        DisplayBrowserCard(browserIndex);
    }

    void DisplayBrowserCard(int index)
    {
        CardData card = PlayerCardCollection.Instance.GetUnlockedCards()[index];
        browserCardImage.sprite = card.cardSprite;
        browserCardNameText.text = card.cardName;
    }

    void AssignCardToSlot()
    {
        if (selectedSlotIndex == -1) return;

        currentDeck[selectedSlotIndex] = PlayerCardCollection.Instance.GetUnlockedCards()[browserIndex];
        selectedSlotIndex = -1;
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