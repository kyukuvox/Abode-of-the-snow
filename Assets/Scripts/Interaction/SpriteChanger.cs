using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite newSprite;       
    public GameObject hoverSprite; 

    private SpriteRenderer spriteRenderer;
    private bool hasBeenClicked = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (hasBeenClicked) return;
        if (hoverSprite != null)
            hoverSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);
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

            if (hoverSprite != null)
                hoverSprite.SetActive(false);
        }
    }
}