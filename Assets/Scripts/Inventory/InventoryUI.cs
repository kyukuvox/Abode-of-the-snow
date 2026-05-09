using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    public Transform slotContainer;
    public GameObject slotPrefab;

    private void Start()
    {
        Inventory.Instance.onItemChangedCallback += UpdateUI;
        StartCoroutine(InitUI());
    }

    IEnumerator InitUI()
    {
        yield return null;
        yield return null;
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (Item item in Inventory.Instance.items)
        {
            GameObject slot = Instantiate(slotPrefab, slotContainer);

            Image itemIcon = slot.transform.Find("ItemIcon")?.GetComponent<Image>();
            if (itemIcon == null)
                itemIcon = slot.transform.Find("Visual/ItemIcon")?.GetComponent<Image>();
            if (itemIcon == null)
                itemIcon = slot.GetComponentInChildren<Image>();

            if (itemIcon != null)
                itemIcon.sprite = item.itemIcon;

            ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
            if (slotUI != null)
                slotUI.Setup(item);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            slotContainer.GetComponent<RectTransform>()
        );
    }
}