using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BadDecisionManager : MonoBehaviour
{
    public static BadDecisionManager Instance;

    public GameObject badDecisionOverlay;
    public GameObject gameOverPanel;
    public Image overlayImage;
    public Sprite[] decisionSprites;

    public float overlayDuration = 2f;
    public float fadeInDuration = 0.05f;
    public float fadeOutDuration = 1f;
    public float gameOverFadeDuration = 1f;

    [System.Serializable]
    public class DecisionSound
    {
        public AudioClip sound;
        [Range(0f, 1f)]
        public float volume = 1f;
    }

    [Header("Sons")]
    public DecisionSound[] decisionSounds;

    [Header("Musique Game Over")]
    public AudioClip gameOverMusic;
    [Range(0f, 1f)]
    public float gameOverMusicVolume = 1f;
    public float musicFadeDuration = 1f;

    [Header("Son bouton Game Over")]
    public AudioClip gameOverButtonSound;
    [Range(0f, 1f)]
    public float gameOverButtonVolume = 1f;

    public int maxLives = 4;
    public int currentLives;
    public bool isGameOver = false;
    public bool isOverlayActive = false;

    private AudioSource musicAudioSource;
    private Button gameOverButton;

    void Awake()
    {
        Instance = this;
        currentLives = maxLives;

        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.spatialBlend = 0f;
        musicAudioSource.volume = 0f;
    }

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverButton = gameOverPanel.GetComponentInChildren<Button>();
            if (gameOverButton != null)
                gameOverButton.onClick.AddListener(OnGameOverButtonClicked);
        }
    }

    void PlayDecisionSound(int index)
    {
        if (decisionSounds == null || index >= decisionSounds.Length) return;
        DecisionSound ds = decisionSounds[index];
        if (ds.sound == null) return;
        SoundSettings.PlaySound(ds.sound, ds.volume, this);
    }

    void OnGameOverButtonClicked()
    {
        SoundSettings.PlaySound(gameOverButtonSound, gameOverButtonVolume, this);
        StartCoroutine(FadeOutMusicThenQuit());
    }

    IEnumerator FadeOutMusicThenQuit()
    {
        float startVolume = musicAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
            yield return null;
        }

        musicAudioSource.volume = 0f;
        musicAudioSource.Stop();

        PauseMenu.Instance.QuitToTitleFromGameOver();
    }

    IEnumerator FadeInMusic()
    {
        if (gameOverMusic == null) yield break;

        musicAudioSource.clip = gameOverMusic;
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();

        float elapsed = 0f;
        float targetVolume = gameOverMusicVolume * SoundSettings.MusicVolume;

        while (elapsed < musicFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; 
            musicAudioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / musicFadeDuration);
            yield return null;
        }

        musicAudioSource.volume = targetVolume;
    }

    public void TriggerBadDecision()
    {
        currentLives--;

        int spriteIndex = maxLives - currentLives - 1;
        if (overlayImage != null && spriteIndex < decisionSprites.Length)
            overlayImage.sprite = decisionSprites[spriteIndex];

        if (currentLives <= 0)
            StartCoroutine(ShowOverlayThenGameOver(spriteIndex));
        else
            StartCoroutine(ShowOverlay(spriteIndex));
    }

    IEnumerator ShowOverlay(int spriteIndex)
    {
        isOverlayActive = true;
        badDecisionOverlay.SetActive(true);

        PlayDecisionSound(spriteIndex);

        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeInDuration));
        yield return new WaitForSeconds(overlayDuration);
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeOutDuration));

        badDecisionOverlay.SetActive(false);
        isOverlayActive = false;
    }

    IEnumerator ShowOverlayThenGameOver(int spriteIndex)
    {
        isOverlayActive = true;
        badDecisionOverlay.SetActive(true);

        PlayDecisionSound(spriteIndex);

        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeInDuration));
        yield return new WaitForSeconds(overlayDuration);

        isOverlayActive = false;
        TriggerGameOver();
    }

    IEnumerator FadeOverlay(float startAlpha, float endAlpha, float duration)
    {
        Color c = overlayImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            overlayImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        overlayImage.color = c;
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        StartCoroutine(FadeInGameOver());
    }

    IEnumerator FadeInGameOver()
    {
        gameOverPanel.SetActive(true);

        CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = gameOverPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < gameOverFadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / gameOverFadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
        Time.timeScale = 0f;

        StartCoroutine(FadeInMusic());
    }
}