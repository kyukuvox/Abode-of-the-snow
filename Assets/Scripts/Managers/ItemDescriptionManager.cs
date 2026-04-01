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
    public float animationSpeed = 5f;
    public float slideOffset = 50f;

    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;
    private RectTransform panelRect;

    void Awake()
    {
        Instance = this;
        panelRect = descriptionPanel.GetComponent<RectTransform>();
    }

    public void ShowItemDescription(Item item)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        itemNameText.text = item.itemName;
        itemSpriteImage.sprite = item.descriptionSprite;

        descriptionPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(AnimateOpen());
        typingCoroutine = StartCoroutine(TypeDescription(item.description));
    }

    IEnumerator AnimateOpen()
    {
        CanvasGroup canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();

        Vector2 startPos = panelRect.anchoredPosition - new Vector2(0, slideOffset);
        Vector2 targetPos = panelRect.anchoredPosition;

        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * animationSpeed;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.anchoredPosition = targetPos;
    }

    IEnumerator AnimateClose()
    {
        CanvasGroup canvasGroup = descriptionPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = descriptionPanel.AddComponent<CanvasGroup>();

        Vector2 startPos = panelRect.anchoredPosition;
        Vector2 targetPos = panelRect.anchoredPosition - new Vector2(0, slideOffset);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * animationSpeed;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = startPos;
        descriptionPanel.SetActive(false);
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
        StopAllCoroutines();
        StartCoroutine(AnimateClose());
    }

    public bool IsActive()
    {
        return descriptionPanel.activeSelf;
    }
}