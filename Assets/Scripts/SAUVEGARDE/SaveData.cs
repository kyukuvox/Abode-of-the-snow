using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public float playerX;
    public float playerY;
    public int currentLives;
    public List<string> inventoryItemNames = new List<string>();
    public List<string> deckCardNames = new List<string>();
    public List<string> unlockedCardNames = new List<string>();
    public List<string> defeatedNPCNames = new List<string>();
    public List<string> pickedUpItemNames = new List<string>();
    public List<string> activatedObjectNames = new List<string>();
    public List<string> npcConsumedItems = new List<string>();
}