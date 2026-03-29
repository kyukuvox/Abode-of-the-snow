using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject menuPanel;
    public GameObject glossairePage;
    public GameObject deckBuilderPage;

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
        isMenuOpen = true;
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        ShowGlossaire();
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
    }

    public void ShowDeckBuilder()
    {
        glossairePage.SetActive(false);
        deckBuilderPage.SetActive(true);
    }
}