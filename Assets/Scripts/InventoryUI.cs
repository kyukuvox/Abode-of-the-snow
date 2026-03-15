using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;        // Glisses-y ton Prefab ItemSlot
    public Transform slotContainer;     // Glisses-y InventoryPanel lui-même

    private Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
        inventory.onItemChangedCallback += UpdateUI; // S'abonne aux changements
    }

    void UpdateUI()
    {
        // Supprime tous les anciens slots
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        // Recrée un slot pour chaque item
        foreach (Item item in inventory.items)
        {
            GameObject slot = Instantiate(slotPrefab, slotContainer);
            // Trouve l'image enfant et applique l'icône de l'item
            Image icon = slot.transform.GetChild(0).GetComponent<Image>();
            icon.sprite = item.itemIcon;
            ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
            slotUI.Setup(item);
        }
    }
}