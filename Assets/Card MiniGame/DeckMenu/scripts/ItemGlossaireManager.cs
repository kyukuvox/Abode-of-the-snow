using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGlossaireManager : MonoBehaviour
{
    public Image itemImage;
    public Text itemNameText;
    public Text itemDescriptionText;

    public Button leftArrowButton;
    public Button rightArrowButton;

    private int currentIndex = 0;
    private List<Item> collectedItems = new List<Item>();

    void Start()
    {
        leftArrowButton.onClick.AddListener(PreviousItem);
        rightArrowButton.onClick.AddListener(NextItem);
    }

    void OnEnable()
    {
        RefreshItems();
    }

    public void RefreshItems()
    {
        if (Inventory.Instance == null)
        {
            Debug.Log("Inventory introuvable !");
            return;
        }

        collectedItems = new List<Item>(Inventory.Instance.items);
        Debug.Log("Items dans le glossaire : " + collectedItems.Count);

        currentIndex = 0;

        if (collectedItems.Count > 0)
            DisplayItem(currentIndex);
        else
            Debug.Log("Aucun item à afficher !");
    }

    void PreviousItem()
    {
        if (collectedItems.Count == 0) return;
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = collectedItems.Count - 1;
        DisplayItem(currentIndex);
    }

    void NextItem()
    {
        if (collectedItems.Count == 0) return;
        currentIndex++;
        if (currentIndex >= collectedItems.Count)
            currentIndex = 0;
        DisplayItem(currentIndex);
    }

    void DisplayItem(int index)
    {
        Item item = collectedItems[index];

        if (itemImage != null)
        {
            itemImage.sprite = item.descriptionSprite;
            itemImage.color = Color.white;
        }

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;
    }
}