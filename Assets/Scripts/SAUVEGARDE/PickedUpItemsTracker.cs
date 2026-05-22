using System.Collections.Generic;
using UnityEngine;

public class PickedUpItemsTracker : MonoBehaviour
{
    public static PickedUpItemsTracker Instance;

    private List<string> pickedUpItems = new List<string>();

    void Awake()
    {
        Instance = this;
    }

    public void AddPickedUpItem(string itemName)
    {
        if (!pickedUpItems.Contains(itemName))
            pickedUpItems.Add(itemName);
    }

    public bool HasPickedUp(string itemName) 
    {
        return pickedUpItems.Contains(itemName);
    }

    public List<string> GetPickedUpItems()
    {
        return pickedUpItems;
    }

    public void LoadPickedUpItems(List<string> items)
    {
        pickedUpItems = new List<string>(items);
        HidePickedUpItems();
    }

    void HidePickedUpItems()
    {
        ItemPickup[] allItems = FindObjectsOfType<ItemPickup>();
        foreach (ItemPickup pickup in allItems)
            if (pickedUpItems.Contains(pickup.item.itemName))
                pickup.gameObject.SetActive(false);
    }
}