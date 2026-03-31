using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject menuPanel;
    public GameObject glossairePage;
    public GameObject deckBuilderPage;
    public GameObject itemGlossairePage;

    private bool isMenuOpen = false;

    void Awake()
    {
        Instance = this;
    }

    public bool IsMenuOpen() { return isMenuOpen; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (PauseMenu.Instance.IsPaused()) return;
            if (DialogueManager.Instance.IsActive()) return;
            if (CardGameManager.Instance.cardGameCanvas.activeSelf) return;

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
        ShowGlossaire();        
        Time.timeScale = 0f;    
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
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
        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(false);
        itemGlossairePage.SetActive(true);

        ItemGlossaireManager itemGlossaire = itemGlossairePage.GetComponent<ItemGlossaireManager>();
        if (itemGlossaire != null)
            itemGlossaire.RefreshItems();
    }

}