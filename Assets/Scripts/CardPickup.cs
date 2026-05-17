using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardPickup : MonoBehaviour
{
    public CardData cardToGive;
    public float bumpScale = 1.2f;
    public float bumpDuration = 0.2f;

    [Header("Son")]
    public AudioClip pickupSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private bool hasBeenClicked = false;
    private HoverParticleManager hoverParticles;
    private Vector3 originalScale;
    private Collider2D col;

    void Start()
    {
        hoverParticles = GetComponent<HoverParticleManager>();
        originalScale = transform.localScale;
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (hasBeenClicked) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        bool isHovered = col != null && col.OverlapPoint(mousePos);

        if (isHovered && hoverParticles != null)
            hoverParticles.Show();
        else if (!isHovered && hoverParticles != null)
            hoverParticles.Hide();

        if (isHovered && Input.GetMouseButtonDown(0))
            Activate();
    }

    void Activate()
    {
        hasBeenClicked = true;

        if (pickupSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = pickupSound;
            tempSource.volume = soundVolume;
            tempSource.spatialBlend = 0f;
            tempSource.Play();
            Destroy(tempAudio, pickupSound.length);
        }

        if (cardToGive != null && PlayerCardCollection.Instance != null)
            PlayerCardCollection.Instance.AddCard(cardToGive);

        if (hoverParticles != null)
            hoverParticles.Hide();

        StartCoroutine(ShowCardReward());
        StartCoroutine(Bump());
    }

    IEnumerator ShowCardReward()
    {
        if (CardGameManager.Instance == null) yield break;
        if (CardGameManager.Instance.cardRewardPanel == null) yield break;

        GameObject panel = CardGameManager.Instance.cardRewardPanel;
        Text rewardText = CardGameManager.Instance.cardRewardText;
        float fadeDuration = CardGameManager.Instance.cardRewardFadeDuration;
        float displayDuration = CardGameManager.Instance.cardRewardDisplayDuration;

        if (rewardText != null)
            rewardText.text = "+1 carte dans la collection";

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        panel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
        panel.SetActive(false);
    }

    IEnumerator Bump()
    {
        float elapsed = 0f;
        float halfDuration = bumpDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * bumpScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale * bumpScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}