using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public Item[] allItems;

    public Item GetItemByName(string name)
    {
        foreach (Item item in allItems)
            if (item.itemName == name) return item;
        return null;
    }
}