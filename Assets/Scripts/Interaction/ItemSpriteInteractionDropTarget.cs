using UnityEngine;

public class ItemSpriteInteractionDropTarget : MonoBehaviour
{
    private ItemSpriteInteraction interaction;

    void Start()
    {
        interaction = GetComponent<ItemSpriteInteraction>();
    }

    public void ReceiveDroppedItem(Item item)
    {
        if (interaction == null)
        {
            Debug.Log("Aucun ItemSpriteInteraction sur ce sprite !");
            return;
        }

        interaction.TryActivateWithItem(item);
    }
}