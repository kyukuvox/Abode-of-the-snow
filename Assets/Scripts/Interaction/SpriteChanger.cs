using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite newSprite;
    private SpriteRenderer spriteRenderer;
    private bool hasBeenClicked = false;
    private HoverParticleManager hoverParticles;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hoverParticles = GetComponent<HoverParticleManager>();
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
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

        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
            hasBeenClicked = true;
            if (hoverParticles != null)
                hoverParticles.Hide();
        }
    }
}