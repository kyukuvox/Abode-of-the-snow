using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const string SAVE_KEY = "SaveData";

    public CardDatabase cardDatabase;
    public ItemDatabase itemDatabase;

    void Awake()
    {
        Instance = this;
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void SaveGame()
    {
        Debug.Log("=== SAVE GAME DÉBUT ===");
        SaveData data = new SaveData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
            Debug.Log("Position sauvegardée : " + data.playerX + ", " + data.playerY);
        }
        else
            Debug.Log("ERREUR : Player introuvable !");

        if (BadDecisionManager.Instance != null)
            data.currentLives = BadDecisionManager.Instance.currentLives;
        else
            Debug.Log("ERREUR : BadDecisionManager introuvable !");

        if (Inventory.Instance != null)
        {
            foreach (Item item in Inventory.Instance.items)
                data.inventoryItemNames.Add(item.itemName);
            Debug.Log("Items sauvegardés : " + data.inventoryItemNames.Count);
        }
        else
            Debug.Log("ERREUR : Inventory introuvable !");

        if (DeckBuilderManager.Instance != null)
        {
            foreach (CardData card in DeckBuilderManager.Instance.GetCurrentDeck())
                data.deckCardNames.Add(card.cardName);
            Debug.Log("Deck sauvegardé : " + data.deckCardNames.Count + " cartes");
        }
        else
            Debug.Log("ERREUR : DeckBuilderManager introuvable !");

        if (PlayerCardCollection.Instance != null)
        {
            foreach (CardData card in PlayerCardCollection.Instance.GetUnlockedCards())
                data.unlockedCardNames.Add(card.cardName);
            Debug.Log("Cartes débloquées sauvegardées : " + data.unlockedCardNames.Count);
        }
        else
            Debug.Log("ERREUR : PlayerCardCollection introuvable !");

        NPCWithItemDialogue[] allNPCs = FindObjectsByType<NPCWithItemDialogue>(FindObjectsSortMode.None);
        foreach (NPCWithItemDialogue npc in allNPCs)
            if (npc.HasBeenDefeated())
                data.defeatedNPCNames.Add(npc.gameObject.name);
        Debug.Log("NPCs vaincus sauvegardés : " + data.defeatedNPCNames.Count);

        if (PickedUpItemsTracker.Instance != null)
        {
            foreach (string itemName in PickedUpItemsTracker.Instance.GetPickedUpItems())
                data.pickedUpItemNames.Add(itemName);
            Debug.Log("Items ramassés sauvegardés : " + data.pickedUpItemNames.Count);
        }
        else
            Debug.Log("ERREUR : PickedUpItemsTracker introuvable !");

        string json = JsonUtility.ToJson(data);
        Debug.Log("JSON généré : " + json);

        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
        Debug.Log("Vérification immédiate : " + PlayerPrefs.HasKey("SaveData"));
        Debug.Log("Valeur sauvegardée : " + PlayerPrefs.GetString("SaveData", "VIDE"));

        // Vérifie que la save est bien écrite
        bool saveExists = PlayerPrefs.HasKey("SaveData");
        Debug.Log("Save écrite dans PlayerPrefs : " + saveExists);
        Debug.Log("=== SAVE GAME FIN ===");
    }

    public void LoadGame()
    {
        if (!HasSave())
        {
            Debug.Log("Aucune save à charger !");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        Debug.Log("JSON chargé : " + json);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Position du joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector2(data.playerX, data.playerY);
            Debug.Log("Position restaurée : " + data.playerX + ", " + data.playerY);
        }

        // Vies
        BadDecisionManager.Instance.currentLives = data.currentLives;
        Debug.Log("Vies restaurées : " + data.currentLives);

        // Inventaire
        Inventory.Instance.items.Clear();
        foreach (string itemName in data.inventoryItemNames)
        {
            Item item = itemDatabase.GetItemByName(itemName);
            if (item != null)
            {
                Inventory.Instance.items.Add(item);
                Debug.Log("Item restauré : " + itemName);
            }
            else
                Debug.Log("Item introuvable : " + itemName);
        }
        // Force la mise à jour de l'UI de l'inventaire
        if (Inventory.Instance.onItemChangedCallback != null)
            Inventory.Instance.onItemChangedCallback.Invoke();

        // Cartes débloquées
        PlayerCardCollection.Instance.ClearAndReload();
        foreach (string cardName in data.unlockedCardNames)
        {
            CardData card = GetCardByName(cardName);
            if (card != null)
                PlayerCardCollection.Instance.AddCard(card);
            else
                Debug.Log("Carte introuvable : " + cardName);
        }

        // Deck
        List<CardData> deck = new List<CardData>();
        foreach (string cardName in data.deckCardNames)
        {
            CardData card = GetCardByName(cardName);
            if (card != null)
                deck.Add(card);
        }
        DeckBuilderManager.Instance.LoadDeck(deck);

        // PNJs vaincus
        NPCWithItemDialogue[] allNPCs = FindObjectsByType<NPCWithItemDialogue>(FindObjectsSortMode.None);
        foreach (NPCWithItemDialogue npc in allNPCs)
            if (data.defeatedNPCNames.Contains(npc.gameObject.name))
                npc.SetDefeated(true);

        // Items ramassés
        if (PickedUpItemsTracker.Instance != null)
            PickedUpItemsTracker.Instance.LoadPickedUpItems(data.pickedUpItemNames);

        Debug.Log("Chargement terminé !");
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }

    CardData GetCardByName(string name)
    {
        foreach (CardData card in cardDatabase.allCards)
            if (card.cardName == name) return card;
        return null;
    }
}