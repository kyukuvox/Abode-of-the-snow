using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public GameObject pausePanel;
    private bool isPaused = false;
    public bool IsPaused() { return isPaused; }
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

    public void QuitToTitleFromGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}