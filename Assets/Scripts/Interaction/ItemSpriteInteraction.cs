using UnityEngine;

public class ItemSpriteInteraction : MonoBehaviour
{
    public Item requiredItem;
    public GameObject spriteToRemove;
    public GameObject hoverSprite;
    public bool consumesItem = true;
    public Sprite activatedSprite; 

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer; 

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
        if (isActivated) return;

        if (hoverSprite != null)
            hoverSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);
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

            if (spriteToRemove != null)
                Destroy(spriteToRemove);

            if (hoverSprite != null)
                hoverSprite.SetActive(false);
        }
        else
        {
            Inventory.Instance.AddItem(item);
            Debug.Log("Il vous faut : " + requiredItem.itemName);
        }
    }
}