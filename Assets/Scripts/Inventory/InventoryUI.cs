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

            Image itemIcon = null;
            Image[] allImages = slot.GetComponentsInChildren<Image>();
            foreach (Image img in allImages)
            {
                if (img.gameObject.name == "ItemIcon")
                {
                    itemIcon = img;
                    break;
                }
            }

            if (itemIcon == null && allImages.Length > 0)
                itemIcon = allImages[0];

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