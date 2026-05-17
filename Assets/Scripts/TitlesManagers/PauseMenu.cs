using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public GameObject pausePanel;
    public float animationSpeed = 5f;
    public float slideOffset = 50f;

    [Header("Sons")]
    public AudioClip openSound;
    [Range(0f, 1f)]
    public float openSoundVolume = 1f;
    public AudioClip closeSound;
    [Range(0f, 1f)]
    public float closeSoundVolume = 1f;
    public AudioClip buttonSound;
    [Range(0f, 1f)]
    public float buttonSoundVolume = 1f;

    private bool isPaused = false;
    private bool isAnimating = false;
    private RectTransform panelRect;

    private const string LOAD_FLAG = "ShouldLoad";

    void Awake()
    {
        Instance = this;
        panelRect = pausePanel.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (DialogueManager.Instance.IsActive()) return;
        if (BadDecisionManager.Instance.isGameOver) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (isAnimating) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = volume;
        tempSource.spatialBlend = 0f;
        tempSource.Play();
        Destroy(tempAudio, clip.length);
    }

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimateOpen());
    }

    public void Resume()
    {
        PlaySound(buttonSound, buttonSoundVolume);
        StopAllCoroutines();
        StartCoroutine(AnimateClose());
    }

    IEnumerator AnimateOpen()
    {
        isAnimating = true;

        PlaySound(openSound, openSoundVolume);

        CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = pausePanel.AddComponent<CanvasGroup>();

        Vector2 startPos = panelRect.anchoredPosition - new Vector2(0, slideOffset);
        Vector2 targetPos = panelRect.anchoredPosition;

        cg.alpha = 0f;
        panelRect.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * animationSpeed;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        cg.alpha = 1f;
        panelRect.anchoredPosition = targetPos;
        Time.timeScale = 0f;
        isAnimating = false;
    }

    IEnumerator AnimateClose()
    {
        isAnimating = true;
        Time.timeScale = 1f;

        PlaySound(closeSound, closeSoundVolume);

        CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = pausePanel.AddComponent<CanvasGroup>();

        Vector2 startPos = panelRect.anchoredPosition;
        Vector2 targetPos = panelRect.anchoredPosition - new Vector2(0, slideOffset);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * animationSpeed;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed);
            yield return null;
        }

        cg.alpha = 0f;
        panelRect.anchoredPosition = startPos;
        isPaused = false;
        pausePanel.SetActive(false);
        isAnimating = false;
    }

    public void QuitToTitle()
    {
        PlaySound(buttonSound, buttonSoundVolume);
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
        SceneManager.LoadScene(0);
    }

    public void QuitToTitleFromGameOver()
    {
        PlaySound(buttonSound, buttonSoundVolume);
        Time.timeScale = 1f;

        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        SceneManager.LoadScene(0);
    }

    public bool IsPaused() { return isPaused; }
}