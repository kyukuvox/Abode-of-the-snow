using UnityEngine;

public class PortalItemActivator : MonoBehaviour
{
    public Item requiredItem;
    public PortalAnimator portalTarget;
    public bool consumesItem = true;
    public Sprite activatedSprite;

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
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
        if (isActivated) return;
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    public void TryActivateWithItem(Item item)
    {
        if (isActivated) return;

        if (item == requiredItem)
        {
            isActivated = true;

            if (activatedSprite != null && spriteRenderer != null)
                spriteRenderer.sprite = activatedSprite;

            if (consumesItem)
            {
                if (Inventory.Instance.onItemChangedCallback != null)
                    Inventory.Instance.onItemChangedCallback.Invoke();
            }
            else
            {
                Inventory.Instance.AddItem(item);
            }

            if (portalTarget != null)
                portalTarget.ActivatePortal();

            if (hoverParticles != null)
                hoverParticles.Hide();
        }
        else
        {
            Inventory.Instance.AddItem(item);
            Debug.Log("Il vous faut : " + requiredItem.itemName);
        }
    }
}