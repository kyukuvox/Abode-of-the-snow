using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [Header("Sons")]
    public AudioClip openSound;
    [Range(0f, 1f)]
    public float openSoundVolume = 1f;
    public AudioClip closeSound;
    [Range(0f, 1f)]
    public float closeSoundVolume = 1f;
    public AudioClip navbarSound;
    [Range(0f, 1f)]
    public float navbarSoundVolume = 1f;
    public AudioClip navigationSound;
    [Range(0f, 1f)]
    public float navigationSoundVolume = 1f;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isAnimating) return;
            if (isMenuOpen)
                CloseMenu();
        }

        if (isMenuOpen && !isAnimating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                NavigateLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                NavigateRight();
        }
    }

    void NavigateLeft()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (glossairePage.activeSelf)
        {
            GlossaireManager glossaire = glossairePage.GetComponent<GlossaireManager>();
            if (glossaire != null)
                glossaire.PreviousCard();
        }
        else if (itemGlossairePage.activeSelf)
        {
            ItemGlossaireManager itemGlossaire = itemGlossairePage.GetComponent<ItemGlossaireManager>();
            if (itemGlossaire != null)
                itemGlossaire.PreviousItem();
        }
        else if (deckBuilderPage.activeSelf)
        {
            if (DeckBuilderManager.Instance != null)
                DeckBuilderManager.Instance.PreviousBrowserCard();
        }
    }

    void NavigateRight()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (glossairePage.activeSelf)
        {
            GlossaireManager glossaire = glossairePage.GetComponent<GlossaireManager>();
            if (glossaire != null)
                glossaire.NextCard();
        }
        else if (itemGlossairePage.activeSelf)
        {
            ItemGlossaireManager itemGlossaire = itemGlossairePage.GetComponent<ItemGlossaireManager>();
            if (itemGlossaire != null)
                itemGlossaire.NextItem();
        }
        else if (deckBuilderPage.activeSelf)
        {
            if (DeckBuilderManager.Instance != null)
                DeckBuilderManager.Instance.NextBrowserCard();
        }
    }

    public void PlayNavigationSound()
    {
        SoundSettings.PlaySound(navigationSound, navigationSoundVolume, this);
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

        SoundSettings.PlaySound(openSound, openSoundVolume, this);

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

        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator AnimateClose()
    {
        isAnimating = true;
        Time.timeScale = 1f;

        SoundSettings.PlaySound(closeSound, closeSoundVolume, this);

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
        SoundSettings.PlaySound(navbarSound, navbarSoundVolume, this);
        glossairePage.SetActive(true);
        deckBuilderPage.SetActive(false);
        itemGlossairePage.SetActive(false);

        GlossaireManager glossaire = glossairePage.GetComponent<GlossaireManager>();
        if (glossaire != null)
            glossaire.RefreshCards();

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowDeckBuilder()
    {
        SoundSettings.PlaySound(navbarSound, navbarSoundVolume, this);
        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(true);
        itemGlossairePage.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ShowItemGlossaire()
    {
        if (Inventory.Instance.items.Count == 0) return;

        SoundSettings.PlaySound(navbarSound, navbarSoundVolume, this);
        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(false);
        itemGlossairePage.SetActive(true);

        ItemGlossaireManager itemGlossaire = itemGlossairePage.GetComponent<ItemGlossaireManager>();
        if (itemGlossaire != null)
            itemGlossaire.RefreshItems();

        EventSystem.current.SetSelectedGameObject(null);
    }

    public bool IsMenuOpen() { return isMenuOpen; }
}