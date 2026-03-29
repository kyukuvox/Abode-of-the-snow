using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public GameObject pausePanel;
    private bool isPaused = false;

    private const string LOAD_FLAG = "ShouldLoad";

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (DialogueManager.Instance.IsActive()) return;
        if (BadDecisionManager.Instance.isGameOver) return;
        if (MenuManager.Instance.IsMenuOpen()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToTitle()
    {
        Debug.Log("=== QUIT TO TITLE APPELÉ ===");
        isPaused = false;
        Time.timeScale = 1f;

        try
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.SaveGame();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur pendant SaveGame : " + e.Message);
        }

        StartCoroutine(LoadTitleAfterSave());
    }

    IEnumerator LoadTitleAfterSave()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Debug.Log("Save présente avant chargement : " + PlayerPrefs.HasKey("SaveData"));
        SceneManager.LoadScene(0);
    }

    public void QuitToTitleFromGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public bool IsPaused() { return isPaused; }
}