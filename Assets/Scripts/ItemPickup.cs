using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    // A DONNER A TOUS LES ITEMS AU SOL !!!!

    public Item item;
    public float pickupRange = 1.5f;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRange;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.IsActive())
            {
                Inventory.Instance.AddItem(item);
                Destroy(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}