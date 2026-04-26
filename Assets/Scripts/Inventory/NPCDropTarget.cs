using UnityEngine;

public class NPCDropTarget : MonoBehaviour
{
    private NPCWithItemDialogue npcDialogue;

    void Start()
    {
        npcDialogue = GetComponent<NPCWithItemDialogue>();
    }

    public void ReceiveDroppedItem(Item item)
    {
        if (npcDialogue == null)
        {
            Debug.Log("Ce PNJ ne peut pas recevoir d'items !");
            return;
        }

        npcDialogue.ReceiveItem(item);
    }
}