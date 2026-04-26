using System.Collections;
using UnityEngine;

public class ItemInteractableSprite : MonoBehaviour
{
    public Item requiredItem;
    public GameObject animatedObject;
    public float targetYOffset = -3f;
    public float descendSpeed = 2f;
    public GameObject hoverSprite;
    public Sprite activatedSprite; 

    private bool isActivated = false;
    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hoverSprite != null)
            hoverSprite.SetActive(false);

        if (animatedObject != null)
            targetPosition = new Vector3(
                animatedObject.transform.position.x,
                animatedObject.transform.position.y + targetYOffset,
                animatedObject.transform.position.z
            );
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

            if (hoverSprite != null)
                hoverSprite.SetActive(false);

            if (Inventory.Instance.onItemChangedCallback != null)
                Inventory.Instance.onItemChangedCallback.Invoke();

            if (animatedObject != null)
                StartCoroutine(DescendObject());
        }
        else
        {
            Inventory.Instance.AddItem(item);
            Debug.Log("Il vous faut : " + requiredItem.itemName);
        }
    }

    IEnumerator DescendObject()
    {
        Vector3 startPos = animatedObject.transform.position;

        while (Vector3.Distance(animatedObject.transform.position, targetPosition) > 0.01f)
        {
            animatedObject.transform.position = Vector3.Lerp(
                animatedObject.transform.position,
                targetPosition,
                Time.deltaTime * descendSpeed
            );
            yield return null;
        }

        animatedObject.transform.position = targetPosition;
    }

    void OnDrawGizmosSelected()
    {
        if (animatedObject != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 target = new Vector3(
                animatedObject.transform.position.x,
                animatedObject.transform.position.y + targetYOffset,
                animatedObject.transform.position.z
            );
            Gizmos.DrawWireCube(target, Vector3.one * 0.5f);
            Gizmos.DrawLine(animatedObject.transform.position, target);
        }
    }
}