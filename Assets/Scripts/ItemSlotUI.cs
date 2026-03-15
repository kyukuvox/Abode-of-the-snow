using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    private Item myItem;
    private Button myButton;

    void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClick);
    }

    // Appelé par InventoryUI pour assigner l'item à ce slot
    public void Setup(Item item)
    {
        myItem = item;
    }

    void OnClick()
    {
        // Trouve le PNJ le plus proche et lui donne l'item
        NPCWithItemDialogue npc = FindNearestNPC();
        if (npc != null)
        {
            npc.ReceiveItem(myItem);
            Inventory.Instance.RemoveItem(myItem);
        }
    }

    NPCWithItemDialogue FindNearestNPC()
    {
        // Cherche tous les PNJs dans la scène
        NPCWithItemDialogue[] allNPCs = FindObjectsOfType<NPCWithItemDialogue>();
        NPCWithItemDialogue nearest = null;
        float minDist = 3f; // Distance max pour donner un item

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        foreach (var npc in allNPCs)
        {
            float dist = Vector2.Distance(player.transform.position, npc.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = npc;
            }
        }
        return nearest;
    }
}