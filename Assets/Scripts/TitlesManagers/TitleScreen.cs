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

    [Header("Vidéo d'intro")]
    public VideoPlayer introVideoPlayer;
    public RawImage videoDisplay;
    public CanvasGroup videoCanvasGroup;
    public CanvasGroup blackPanel;

    private const string SAVE_KEY = "SaveData";
    private const string LOAD_FLAG = "ShouldLoad";

    void Awake()
    {
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;

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
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        bool hasSave = PlayerPrefs.HasKey(SAVE_KEY);

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.interactable = false;
        }

        Invoke("EnableButtons", 0.5f);
    }

    void EnableButtons()
    {
        if (playButton != null)
            playButton.interactable = true;

        if (continueButton != null)
            continueButton.interactable = PlayerPrefs.HasKey(SAVE_KEY);

        if (quitButton != null)
            quitButton.interactable = true;
    }

    public void PlayGame()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(LOAD_FLAG);
        PlayerPrefs.Save();

        StartCoroutine(PlayIntroThenLoad());
    }

    IEnumerator PlayIntroThenLoad()
    {
        if (playButton != null) playButton.interactable = false;
        if (continueButton != null) continueButton.interactable = false;
        if (quitButton != null) quitButton.interactable = false;

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
        PlayerPrefs.SetInt(LOAD_FLAG, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}