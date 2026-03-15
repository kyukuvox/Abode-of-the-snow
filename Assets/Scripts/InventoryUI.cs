using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotContainer;

    private Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
        inventory.onItemChangedCallback += UpdateUI;
    }

    void UpdateUI()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (Item item in inventory.items)
        {
            GameObject slot = Instantiate(slotPrefab, slotContainer);

            Image icon = slot.transform.GetChild(0).GetComponent<Image>();
            icon.sprite = item.itemIcon;

            ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
            slotUI.Setup(item);

            // Initialise la position de base après un frame
            // pour laisser le Layout Group placer le slot d'abord
            StartCoroutine(InitSlotPosition(slot));
        }
    }

    IEnumerator InitSlotPosition(GameObject slot)
    {
        yield return null; // Attend un frame
        ItemSlotHover hover = slot.GetComponent<ItemSlotHover>();
        if (hover != null)
            hover.InitBasePosition();
    }
}