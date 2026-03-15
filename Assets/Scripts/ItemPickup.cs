using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // L'item ScriptableObject à donner au joueur

    // Ajoute un Collider2D sur cet objet et coche "Is Trigger"
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory.Instance.AddItem(item);
            Destroy(gameObject); // Supprime l'objet du sol
        }
    }
}