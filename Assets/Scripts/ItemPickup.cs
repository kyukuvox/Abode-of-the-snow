using UnityEngine;

public class ItemPickup : MonoBehaviour
{
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
            // Ne ramasse l'item que si aucun dialogue n'est en cours
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