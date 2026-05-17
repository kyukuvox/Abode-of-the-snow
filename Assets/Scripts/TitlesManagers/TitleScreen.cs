using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    public Button continueButton;
    public Button playButton;
    public Button quitButton;
    public Button optionButton;

    [Header("Page Options")]
    public GameObject optionsPage;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button returnButton;

    [Header("Boutons principaux")]
    public GameObject continueButtonObj;
    public GameObject playButtonObj;
    public GameObject quitButtonObj;
    public GameObject optionButtonObj;

    [Header("Vidéo d'intro")]
    public VideoPlayer introVideoPlayer;
    public RawImage videoDisplay;
    public CanvasGroup videoCanvasGroup;
    public CanvasGroup blackPanel;

    [Header("Musique")]
    public AudioClip titleMusic;
    [Range(0f, 1f)]
    public float titleMusicVolume = 1f;
    public float musicFadeInDuration = 1f;

    [Header("Son")]
    public AudioClip buttonSound;
    [Range(0f, 1f)]
    public float buttonSoundVolume = 1f;

    private AudioSource musicAudioSource;

    private const string SAVE_KEY = "SaveData";
    private const string LOAD_FLAG = "ShouldLoad";

    void Awake()
    {
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
        if (optionButton != null) optionButton.interactable = false;

        if (videoCanvasGroup != null)
        {
            videoCanvasGroup.alpha = 0f;
            videoCanvasGroup.gameObject.SetActive(false);
        }

        if (blackPanel != null)
        {
            blackPanel.alpha = 0f;
            blackPanel.gameObject.SetActive(false);
        }

        if (optionsPage != null)
            optionsPage.SetActive(false);

        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.spatialBlend = 0f;
        musicAudioSource.volume = 0f;
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.interactable = false;
        }

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

        if (titleMusic != null)
            StartCoroutine(FadeInMusic());

        Invoke("EnableButtons", 0.5f);
    }

    void PlayButtonSound()
    {
        if (buttonSound == null) return;
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = buttonSound;
        tempSource.volume = buttonSoundVolume * SoundSettings.SFXVolume;
        tempSource.spatialBlend = 0f;
        tempSource.Play();
        Destroy(tempAudio, buttonSound.length);
    }

    IEnumerator FadeInMusic()
    {
        musicAudioSource.clip = titleMusic;
        musicAudioSource.Play();

        float elapsed = 0f;
        float targetVolume = titleMusicVolume * SoundSettings.MusicVolume;

        while (elapsed < musicFadeInDuration)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / musicFadeInDuration);
            yield return null;
        }
        musicAudioSource.volume = targetVolume;
    }

    IEnumerator FadeOutMusic()
    {
        float startVolume = musicAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < musicFadeInDuration)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeInDuration);
            yield return null;
        }

        musicAudioSource.volume = 0f;
        musicAudioSource.Stop();
    }

    void EnableButtons()
    {
        if (playButton != null) playButton.interactable = true;
        if (continueButton != null)
            continueButton.interactable = PlayerPrefs.HasKey(SAVE_KEY);
        if (quitButton != null) quitButton.interactable = true;
        if (optionButton != null) optionButton.interactable = true;
    }

    void ShowMainButtons()
    {
        if (continueButtonObj != null) continueButtonObj.SetActive(true);
        if (playButtonObj != null) playButtonObj.SetActive(true);
        if (quitButtonObj != null) quitButtonObj.SetActive(true);
        if (optionButtonObj != null) optionButtonObj.SetActive(true);
        if (optionsPage != null) optionsPage.SetActive(false);
    }

    public void OpenOptions()
    {
        PlayButtonSound();

        if (continueButtonObj != null) continueButtonObj.SetActive(false);
        if (playButtonObj != null) playButtonObj.SetActive(false);
        if (quitButtonObj != null) quitButtonObj.SetActive(false);
        if (optionButtonObj != null) optionButtonObj.SetActive(false);

        if (optionsPage != null) optionsPage.SetActive(true);

        if (musicSlider != null) musicSlider.value = SoundSettings.MusicVolume;
        if (sfxSlider != null) sfxSlider.value = SoundSettings.SFXVolume;
    }

    public void CloseOptions()
    {
        PlayButtonSound();
        SoundSettings.SaveSettings();
        if (optionsPage != null) optionsPage.SetActive(false);
        ShowMainButtons();
    }

    void OnMusicVolumeChanged(float value)
    {
        SoundSettings.SetMusicVolume(value);
        if (musicAudioSource != null && musicAudioSource.isPlaying)
            musicAudioSource.volume = titleMusicVolume * value;
    }

    void OnSFXVolumeChanged(float value)
    {
        SoundSettings.SetSFXVolume(value);
    }

    public void PlayGame()
    {
        PlayButtonSound();
        StartCoroutine(FadeOutMusic());
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(LOAD_FLAG);
        PlayerPrefs.DeleteKey("CardGameTutorialShown");
        PlayerPrefs.Save();
        StartCoroutine(PlayIntroThenLoad());
    }

    IEnumerator PlayIntroThenLoad()
    {
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
        if (optionButton != null) optionButton.interactable = false;

        if (introVideoPlayer != null && videoCanvasGroup != null)
        {
            videoCanvasGroup.gameObject.SetActive(true);

            introVideoPlayer.Prepare();
            yield return new WaitUntil(() => introVideoPlayer.isPrepared);

            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                videoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            videoCanvasGroup.alpha = 1f;

            introVideoPlayer.Play();
            yield return new WaitUntil(() => !introVideoPlayer.isPlaying);

            if (blackPanel != null)
            {
                blackPanel.gameObject.SetActive(true);
                elapsed = 0f;
                while (elapsed < 0.5f)
                {
                    elapsed += Time.deltaTime;
                    blackPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                    yield return null;
                }
                blackPanel.alpha = 1f;
            }
        }

        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;
        PlayButtonSound();
        StartCoroutine(FadeOutMusic());
        PlayerPrefs.SetInt(LOAD_FLAG, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Application.Quit();
    }
}