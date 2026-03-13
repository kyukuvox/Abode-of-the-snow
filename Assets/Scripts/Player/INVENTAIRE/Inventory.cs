using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public List<InventoryItem> items = new List<InventoryItem>();

    //void Awake() => Instance = this;
    void Awake()
    {
        Instance = this;
        Debug.Log("Inventory initialisé avec " + items.Count + " items");
    }

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
        InventoryUI.Instance.RefreshUI();
    }
}
