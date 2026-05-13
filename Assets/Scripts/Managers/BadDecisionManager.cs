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
    public int maxLives = 4;
    public int currentLives;
    public bool isGameOver = false;
    public bool isOverlayActive = false;

    void Awake()
    {
        Instance = this;
        currentLives = maxLives;
    }

    public void TriggerBadDecision()
    {
        currentLives--;

        int spriteIndex = maxLives - currentLives - 1;
        if (overlayImage != null && spriteIndex < decisionSprites.Length)
            overlayImage.sprite = decisionSprites[spriteIndex];

        if (currentLives <= 0)
            StartCoroutine(ShowOverlayThenGameOver());
        else
            StartCoroutine(ShowOverlay());
    }

    IEnumerator ShowOverlay()
    {
        isOverlayActive = true;
        badDecisionOverlay.SetActive(true);

        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeInDuration));
        yield return new WaitForSeconds(overlayDuration);
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeOutDuration));

        badDecisionOverlay.SetActive(false);
        isOverlayActive = false;
    }

    IEnumerator ShowOverlayThenGameOver()
    {
        isOverlayActive = true;
        badDecisionOverlay.SetActive(true);

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
    }
}