using UnityEngine;
using UnityEngine.UI;
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

        RefreshBrowser();
        UpdateSlotsDisplay();
    }

    void RefreshBrowser()
    {
        if (PlayerCardCollection.Instance == null)
        {
            Debug.Log("PlayerCardCollection introuvable !");
            return;
        }

        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        Debug.Log("Cartes disponibles dans le navigateur : " + cards.Count);

        if (cards.Count > 0)
            DisplayBrowserCard(0);
        else
            Debug.Log("Aucune carte disponible !");
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
        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        if (cards.Count == 0) return;
        browserIndex--;
        if (browserIndex < 0)
            browserIndex = cards.Count - 1;
        DisplayBrowserCard(browserIndex);
    }

    void NextBrowserCard()
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
        Debug.Log("Carte : " + card.cardName);
        Debug.Log("cardSprite : " + (card.cardSprite != null ? "OK" : "NULL"));
        Debug.Log("browserCardImage : " + (browserCardImage != null ? "OK" : "NULL"));
        Debug.Log("browserCardImage.sprite avant : " + (browserCardImage.sprite != null ? browserCardImage.sprite.name : "NULL"));

        if (browserCardImage != null)
        {
            browserCardImage.enabled = true;
            browserCardImage.gameObject.SetActive(true); // ← force l'activation
            browserCardImage.sprite = card.cardSprite;
            browserCardImage.color = Color.white;
            Debug.Log("Après activation - enabled : " + browserCardImage.enabled);
            Debug.Log("Après activation - gameObject active : " + browserCardImage.gameObject.activeSelf);
            Debug.Log("Parent actif : " + browserCardImage.transform.parent.gameObject.activeSelf);
        }

        if (browserCardNameText != null)
            browserCardNameText.text = card.cardName;
    }

    void AssignCardToSlot()
    {
        if (selectedSlotIndex == -1)
        {
            Debug.Log("Aucun slot sélectionné !");
            return;
        }

        List<CardData> cards = PlayerCardCollection.Instance.GetUnlockedCards();
        if (cards.Count == 0) return;

        currentDeck[selectedSlotIndex] = cards[browserIndex];
        Debug.Log("Carte assignée : " + cards[browserIndex].cardName + " → slot " + selectedSlotIndex);
        selectedSlotIndex = -1;
        UpdateSlotsDisplay();
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
        Debug.Log("GetCurrentDeck : " + deck.Count + " cartes");
        return deck;
    }
}