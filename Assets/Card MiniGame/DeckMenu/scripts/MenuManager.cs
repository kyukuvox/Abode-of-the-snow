using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject menuPanel;
    public GameObject glossairePage;
    public GameObject deckBuilderPage;
    public GameObject itemGlossairePage;
    public GameObject itemGlossaireButton;

    public float animationSpeed = 5f;
    public float slideOffset = 50f;

    private bool isMenuOpen = false;
    private RectTransform panelRect;
    private bool isAnimating = false;

    void Awake()
    {
        Instance = this;
        panelRect = menuPanel.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (DialogueManager.Instance.IsActive()) return;
            if (ItemDescriptionManager.Instance.IsActive()) return;
            if (PauseMenu.Instance.IsPaused()) return;
            if (CardGameManager.Instance.cardGameCanvas.activeSelf) return;
            if (isAnimating) return;

            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (DialogueManager.Instance.IsActive()) return;
        if (CardGameManager.Instance.cardGameCanvas.activeSelf) return;

        isMenuOpen = true;
        menuPanel.SetActive(true);

        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(false);
        itemGlossairePage.SetActive(false);

        if (itemGlossaireButton != null)
            itemGlossaireButton.GetComponent<Button>().interactable =
                Inventory.Instance.items.Count > 0;

        if (Inventory.Instance.items.Count > 0)
            ShowItemGlossaire();
        else
            ShowGlossaire();

        StopAllCoroutines();
        StartCoroutine(AnimateOpen());
    }

    public void CloseMenu()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateClose());
    }

    IEnumerator AnimateOpen()
    {
        isAnimating = true;

        CanvasGroup canvasGroup = menuPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = menuPanel.AddComponent<CanvasGroup>();

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
        Time.timeScale = 0f;
        isAnimating = false;
    }

    IEnumerator AnimateClose()
    {
        isAnimating = true;
        Time.timeScale = 1f;

        CanvasGroup canvasGroup = menuPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = menuPanel.AddComponent<CanvasGroup>();

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
        isMenuOpen = false;
        menuPanel.SetActive(false);
        isAnimating = false;
    }

    public void ShowGlossaire()
    {
        glossairePage.SetActive(true);
        deckBuilderPage.SetActive(false);
        itemGlossairePage.SetActive(false);

        GlossaireManager glossaire = glossairePage.GetComponent<GlossaireManager>();
        if (glossaire != null)
            glossaire.RefreshCards();
    }

    public void ShowDeckBuilder()
    {
        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(true);
        itemGlossairePage.SetActive(false);
    }

    public void ShowItemGlossaire()
    {
        if (Inventory.Instance.items.Count == 0) return;

        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(false);
        itemGlossairePage.SetActive(true);

        ItemGlossaireManager itemGlossaire = itemGlossairePage.GetComponent<ItemGlossaireManager>();
        if (itemGlossaire != null)
            itemGlossaire.RefreshItems();
    }

    public bool IsMenuOpen() { return isMenuOpen; }
}