using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance; // Accès global (Singleton)

    public List<Item> items = new List<Item>();

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback; // Notifie l'UI quand l'inventaire change

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(Item item)
    {
        items.Add(item);

        // Prévient l'UI qu'il faut se mettre à jour
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
