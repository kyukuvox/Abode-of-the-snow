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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distance = Vector2.Distance(
            player.transform.position,
            transform.position
        );

        if (distance <= npcDialogue.interactionRange)
        {
            npcDialogue.ReceiveItem(item);
        }
        else
        {
            Debug.Log("Trop loin du PNJ !");
        }
    }
}