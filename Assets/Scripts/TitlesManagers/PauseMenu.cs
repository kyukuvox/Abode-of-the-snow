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

    [Header("Boutons principaux")]
    public GameObject resumeButton;
    public GameObject optionButton;
    public GameObject quitButton;

    [Header("Page Options")]
    public GameObject optionsPage;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button returnButton;

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

        if (optionsPage != null)
            optionsPage.SetActive(false);
    }

    void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.value = SoundSettings.MusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = SoundSettings.SFXVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        if (returnButton != null)
            returnButton.onClick.AddListener(CloseOptions);
    }

    void Update()
    {
        if (!isPaused && optionsPage != null && optionsPage.activeSelf)
            optionsPage.SetActive(false);

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

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        if (optionsPage != null)
            optionsPage.SetActive(false);

        ShowMainButtons();
        StopAllCoroutines();
        StartCoroutine(AnimateOpen());
    }

    public void Resume()
    {
        SoundSettings.PlaySound(buttonSound, buttonSoundVolume, this);
        StopAllCoroutines();
        StartCoroutine(AnimateClose());
    }

    public void OpenOptions()
    {
        SoundSettings.PlaySound(buttonSound, buttonSoundVolume, this);

        if (resumeButton != null) resumeButton.SetActive(false);
        if (optionButton != null) optionButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);

        if (optionsPage != null) optionsPage.SetActive(true);

        if (musicSlider != null) musicSlider.value = SoundSettings.MusicVolume;
        if (sfxSlider != null) sfxSlider.value = SoundSettings.SFXVolume;
    }

    public void CloseOptions()
    {
        SoundSettings.PlaySound(buttonSound, buttonSoundVolume, this);
        SoundSettings.SaveSettings();

        if (optionsPage != null) optionsPage.SetActive(false);

        ShowMainButtons();
    }

    void ShowMainButtons()
    {
        if (resumeButton != null) resumeButton.SetActive(true);
        if (optionButton != null) optionButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);
        if (optionsPage != null) optionsPage.SetActive(false);
    }

    void OnMusicVolumeChanged(float value)
    {
        SoundSettings.SetMusicVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        SoundSettings.SetSFXVolume(value);
    }

    IEnumerator AnimateOpen()
    {
        isAnimating = true;

        SoundSettings.PlaySound(openSound, openSoundVolume, this);

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

        SoundSettings.PlaySound(closeSound, closeSoundVolume, this);

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
        SoundSettings.PlaySound(buttonSound, buttonSoundVolume, this);
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
        SoundSettings.PlaySound(buttonSound, buttonSoundVolume, this);
        Time.timeScale = 1f;

        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        SceneManager.LoadScene(0);
    }

    public bool IsPaused() { return isPaused; }
}