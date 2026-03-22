using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    private Item myItem;
    private Button myButton;

    void Awake()
    {
        myButton = GetComponentInChildren<Button>();

        if (myButton == null)
            myButton = gameObject.AddComponent<Button>();

        myButton.onClick.AddListener(OnClick);
    }

    public void Setup(Item item)
    {
        myItem = item;
    }


    void OnClick()
    {
        NPCWithItemDialogue npc = FindNearestNPC();
        if (npc != null)
        {
            npc.ReceiveItem(myItem);
        }
    }

    NPCWithItemDialogue FindNearestNPC()
    {
        NPCWithItemDialogue[] allNPCs = FindObjectsOfType<NPCWithItemDialogue>();
        NPCWithItemDialogue nearest = null;
        float minDist = 3f; 

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