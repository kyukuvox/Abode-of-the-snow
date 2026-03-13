using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemId;       // ex: "sword", "scroll", "staff"
    public string displayName;  // ex: "Épée", "Parchemin", "Bâton"
    public Sprite icon;         // l'icône affichée dans l'UI
}