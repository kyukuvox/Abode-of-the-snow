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

    [Header("Vidéos")]
    public VideoPlayer startupVideoPlayer;
    public VideoPlayer loopVideoPlayer;
    public VideoPlayer playIntroVideoPlayer;
    public RawImage videoDisplay;
    public CanvasGroup videoCanvasGroup;
    public CanvasGroup blackPanel;
    public float buttonsFadeInDuration = 1f;

    [Header("Timeouts vidéo")]
    public float videoPrepareTimeout = 10f;
    public float videoPlayTimeout = 60f;

    [Header("Canvas boutons")]
    public CanvasGroup buttonsCanvasGroup;

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
    private RenderTexture renderTexture;

    private const string SAVE_KEY = "SaveData";
    private const string LOAD_FLAG = "ShouldLoad";

    void Awake()
    {
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
        if (optionButton != null) optionButton.interactable = false;

        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.alpha = 0f;
            buttonsCanvasGroup.interactable = false;
            buttonsCanvasGroup.blocksRaycasts = false;
        }

        if (videoCanvasGroup != null)
        {
            videoCanvasGroup.alpha = 0f;
            videoCanvasGroup.gameObject.SetActive(false);
        }

        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(true);
            blackPanel.alpha = 1f;
        }

        if (optionsPage != null)
            optionsPage.SetActive(false);

        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.spatialBlend = 0f;
        musicAudioSource.volume = 0f;

        renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        if (startupVideoPlayer != null)
        {
            startupVideoPlayer.Stop();
            startupVideoPlayer.targetTexture = renderTexture;
        }
        if (loopVideoPlayer != null)
        {
            loopVideoPlayer.Stop();
            loopVideoPlayer.targetTexture = renderTexture;
        }
        if (playIntroVideoPlayer != null)
        {
            playIntroVideoPlayer.Stop();
            playIntroVideoPlayer.targetTexture = renderTexture;
        }
        if (videoDisplay != null)
            videoDisplay.texture = renderTexture;
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

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

        StartCoroutine(VideoSequence());
    }

    IEnumerator WaitForPrepare(VideoPlayer vp)
    {
        float elapsed = 0f;
        while (!vp.isPrepared && elapsed < videoPrepareTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!vp.isPrepared)
            Debug.LogWarning("VideoPlayer timeout : " + vp.name);
    }

    IEnumerator VideoSequence()
    {
        if (startupVideoPlayer != null && videoCanvasGroup != null)
        {
            videoCanvasGroup.gameObject.SetActive(true);
            videoCanvasGroup.alpha = 0f;

            startupVideoPlayer.Prepare();
            yield return StartCoroutine(WaitForPrepare(startupVideoPlayer));

            startupVideoPlayer.Play();

            int frameWait = 0;
            while (!startupVideoPlayer.isPlaying && frameWait < 10)
            {
                frameWait++;
                yield return null;
            }

            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                blackPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / 0.3f);
                videoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.3f);
                yield return null;
            }
            blackPanel.alpha = 0f;
            videoCanvasGroup.alpha = 1f;

            elapsed = 0f;
            float minDuration = (float)startupVideoPlayer.length;

            while (elapsed < minDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            startupVideoPlayer.Stop();
        }

        if (loopVideoPlayer != null)
        {
            loopVideoPlayer.isLooping = true;
            loopVideoPlayer.Prepare();
            yield return StartCoroutine(WaitForPrepare(loopVideoPlayer));
            loopVideoPlayer.Play();
        }

        StartCoroutine(FadeInMusic());
        StartCoroutine(FadeInButtons());
    }

    IEnumerator FadeInButtons()
    {
        if (buttonsCanvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < buttonsFadeInDuration)
        {
            elapsed += Time.deltaTime;
            buttonsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / buttonsFadeInDuration);
            yield return null;
        }
        buttonsCanvasGroup.alpha = 1f;

        buttonsCanvasGroup.interactable = true;
        buttonsCanvasGroup.blocksRaycasts = true;

        if (playButton != null) playButton.interactable = true;
        if (continueButton != null)
            continueButton.interactable = PlayerPrefs.HasKey(SAVE_KEY);
        if (quitButton != null) quitButton.interactable = true;
        if (optionButton != null) optionButton.interactable = true;
    }

    IEnumerator FadeOutButtons(float duration = 0.3f)
    {
        if (buttonsCanvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            buttonsCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        buttonsCanvasGroup.alpha = 0f;
        buttonsCanvasGroup.interactable = false;
        buttonsCanvasGroup.blocksRaycasts = false;
    }

    IEnumerator FadeInMusic()
    {
        if (titleMusic == null) yield break;

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

        yield return StartCoroutine(FadeOutButtons(0.3f));

        if (loopVideoPlayer != null)
            loopVideoPlayer.Stop();

        if (playIntroVideoPlayer != null && videoCanvasGroup != null)
        {
            playIntroVideoPlayer.Prepare();
            yield return StartCoroutine(WaitForPrepare(playIntroVideoPlayer));

            playIntroVideoPlayer.Play();

            int frameWait = 0;
            while (!playIntroVideoPlayer.isPlaying && frameWait < 10)
            {
                frameWait++;
                yield return null;
            }

            float elapsed = 0f;
            float duration = (float)playIntroVideoPlayer.length;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            playIntroVideoPlayer.Stop();
        }

        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blackPanel.alpha = 1f;
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
        StartCoroutine(LoadGameWithFade());
    }

    IEnumerator LoadGameWithFade()
    {
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;
        if (optionButton != null) optionButton.interactable = false;

        yield return StartCoroutine(FadeOutButtons(0.3f));

        if (loopVideoPlayer != null)
            loopVideoPlayer.Stop();

        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                blackPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                yield return null;
            }
            blackPanel.alpha = 1f;
        }

        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Application.Quit();
    }
}