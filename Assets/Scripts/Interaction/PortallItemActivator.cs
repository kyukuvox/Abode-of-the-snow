using UnityEngine;

public class PortalItemActivator : MonoBehaviour
{
    public Item requiredItem;        
    public PortalAnimator portalTarget; 
    public GameObject hoverSprite;   
    public bool consumesItem = true;   

    private bool isActivated = false;

    void Start()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (isActivated) return;

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
        if (isActivated) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;

        if (Inventory.Instance.items.Contains(requiredItem))
        {
            isActivated = true;

            if (consumesItem)
                Inventory.Instance.RemoveItem(requiredItem);

            if (portalTarget != null)
                portalTarget.ActivatePortal();

            if (hoverSprite != null)
                hoverSprite.SetActive(false);
        }
        else
        {
            Debug.Log("Il vous faut : " + requiredItem.itemName);
        }
    }
}