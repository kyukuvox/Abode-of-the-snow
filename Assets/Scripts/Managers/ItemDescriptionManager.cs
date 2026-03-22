using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionManager : MonoBehaviour
{
    public static ItemDescriptionManager Instance;

    public GameObject descriptionPanel;
    public Text itemNameText;
    public Text itemDescriptionText;
    public Image itemSpriteImage;

    public float typingSpeed = 0.05f;
    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;
    }

    public void ShowItemDescription(Item item)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        itemNameText.text = item.itemName;
        itemSpriteImage.sprite = item.descriptionSprite;

        descriptionPanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeDescription(item.description));
    }

    IEnumerator TypeDescription(string text)
    {
        isTyping = true;
        currentFullText = text;
        itemDescriptionText.text = "";

        foreach (char letter in text)
        {
            itemDescriptionText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void ClosePanel()
    {
        if (isTyping) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
        descriptionPanel.SetActive(false);
    }

    public bool IsActive()
    {
        return descriptionPanel.activeSelf;
    }
}