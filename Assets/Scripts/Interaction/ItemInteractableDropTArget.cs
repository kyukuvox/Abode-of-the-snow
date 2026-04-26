using UnityEngine;

public class ItemInteractableDropTarget : MonoBehaviour
{
    private ItemInteractableSprite interactable;

    void Start()
    {
        interactable = GetComponent<ItemInteractableSprite>();
    }

    public void ReceiveDroppedItem(Item item)
    {
        if (interactable == null)
        {
            Debug.Log("Aucun ItemInteractableSprite sur ce sprite !");
            return;
        }

        interactable.TryActivateWithItem(item);
    }
}