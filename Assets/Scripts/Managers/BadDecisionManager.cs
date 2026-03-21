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
    public int maxLives = 4;
    private int currentLives;
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
        yield return new WaitForSeconds(overlayDuration);
        badDecisionOverlay.SetActive(false);
        isOverlayActive = false;
    }

    IEnumerator ShowOverlayThenGameOver()
    {
        isOverlayActive = true;
        badDecisionOverlay.SetActive(true);
        yield return new WaitForSeconds(overlayDuration);
        badDecisionOverlay.SetActive(false);
        isOverlayActive = false;
        TriggerGameOver();
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}