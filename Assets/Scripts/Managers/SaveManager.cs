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
        {
            if (npc.HasBeenDefeated())
                data.defeatedNPCNames.Add(npc.gameObject.name);

            foreach (string itemName in npc.GetConsumedItemNames())
                data.npcConsumedItems.Add(npc.gameObject.name + "|" + itemName);
        }
        Debug.Log("NPCs vaincus sauvegardés : " + data.defeatedNPCNames.Count);
        Debug.Log("Items consommés NPC sauvegardés : " + data.npcConsumedItems.Count);

        if (PickedUpItemsTracker.Instance != null)
        {
            foreach (string itemName in PickedUpItemsTracker.Instance.GetPickedUpItems())
                data.pickedUpItemNames.Add(itemName);
            Debug.Log("Items ramassés sauvegardés : " + data.pickedUpItemNames.Count);
        }
        else
            Debug.Log("ERREUR : PickedUpItemsTracker introuvable !");

        if (ActivatedObjectsTracker.Instance != null)
        {
            foreach (string name in ActivatedObjectsTracker.Instance.GetActivatedObjects())
                data.activatedObjectNames.Add(name);
            Debug.Log("Objets activés sauvegardés : " + data.activatedObjectNames.Count);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
        Debug.Log("Save écrite dans PlayerPrefs : " + PlayerPrefs.HasKey("SaveData"));
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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector2(data.playerX, data.playerY);
            Debug.Log("Position restaurée : " + data.playerX + ", " + data.playerY);
        }

        BadDecisionManager.Instance.currentLives = data.currentLives;
        Debug.Log("Vies restaurées : " + data.currentLives);

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
        if (Inventory.Instance.onItemChangedCallback != null)
            Inventory.Instance.onItemChangedCallback.Invoke();

        PlayerCardCollection.Instance.ClearAndReload();
        foreach (string cardName in data.unlockedCardNames)
        {
            CardData card = GetCardByName(cardName);
            if (card != null)
                PlayerCardCollection.Instance.AddCard(card);
            else
                Debug.Log("Carte introuvable : " + cardName);
        }

        List<CardData> deck = new List<CardData>();
        foreach (string cardName in data.deckCardNames)
        {
            CardData card = GetCardByName(cardName);
            if (card != null)
                deck.Add(card);
        }
        DeckBuilderManager.Instance.LoadDeck(deck);

        NPCWithItemDialogue[] allNPCs = FindObjectsByType<NPCWithItemDialogue>(FindObjectsSortMode.None);
        foreach (NPCWithItemDialogue npc in allNPCs)
            if (data.defeatedNPCNames.Contains(npc.gameObject.name))
                npc.SetDefeated(true);

        foreach (string entry in data.npcConsumedItems)
        {
            string[] parts = entry.Split('|');
            if (parts.Length != 2) continue;

            string npcName = parts[0];
            string itemName = parts[1];

            foreach (NPCWithItemDialogue npc in allNPCs)
            {
                if (npc.gameObject.name == npcName)
                {
                    Item item = itemDatabase.GetItemByName(itemName);
                    if (item != null)
                        npc.LoadConsumedItem(item);
                }
            }
        }

        if (PickedUpItemsTracker.Instance != null)
            PickedUpItemsTracker.Instance.LoadPickedUpItems(data.pickedUpItemNames);

        if (ActivatedObjectsTracker.Instance != null)
            ActivatedObjectsTracker.Instance.LoadActivatedObjects(data.activatedObjectNames);

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

        foreach (CardData card in cardDatabase.rewardCards)
            if (card.cardName == name) return card;

        Debug.Log("Carte introuvable : " + name);
        return null;
    }
}