using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Joueur
    public float playerX;
    public float playerY;

    // Vies
    public int currentLives;

    // Inventaire
    public List<string> inventoryItemNames = new List<string>();

    // Deck constitué
    public List<string> deckCardNames = new List<string>();

    // Cartes débloquées
    public List<string> unlockedCardNames = new List<string>();

    // PNJs vaincus
    public List<string> defeatedNPCNames = new List<string>();

    // Items ramassés
    public List<string> pickedUpItemNames = new List<string>();
}