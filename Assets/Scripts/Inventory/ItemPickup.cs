using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    //A METTRE SUR TOUS LES ITEMS !!!

    public Item item;
    public GameObject hoverSprite;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    void OnMouseEnter()
    {
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
        if (!DialogueManager.Instance.IsActive())
        {
            Inventory.Instance.AddItem(item);
            ItemDescriptionManager.Instance.ShowItemDescription(item);
            Destroy(gameObject);
        }
    }
}