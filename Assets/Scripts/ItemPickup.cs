using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    // à mettre sur chaques items !!!

    public Item item;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnMouseDown()
    {
        if (!DialogueManager.Instance.IsActive())
        {
            Inventory.Instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}