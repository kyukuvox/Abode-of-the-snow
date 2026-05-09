using System.Collections;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite newSprite;
    public float bumpScale = 1.2f;
    public float bumpDuration = 0.2f;

    private SpriteRenderer spriteRenderer;
    private bool hasBeenClicked = false;
    private HoverParticleManager hoverParticles;
    private Vector3 originalScale;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();
        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;
        if (hasBeenClicked) return;
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    void OnMouseDown()
    {
        if (hasBeenClicked) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;

        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
            hasBeenClicked = true;
            if (hoverParticles != null)
                hoverParticles.Hide();
        }

        StartCoroutine(Bump());
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