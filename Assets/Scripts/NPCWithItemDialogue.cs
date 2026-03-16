using UnityEngine;
using System.Collections.Generic;

public class NPCWithItemDialogue : NPCInteraction
{

    //A DONNER a TOUS LES PNJS !

    [System.Serializable]
    public class ItemDialoguePair
    {
        public Item item;           
        public DialogueData dialogue; 
        public bool consumesItem = true;
    }

    public List<ItemDialoguePair> itemDialogues;

    public void ReceiveItem(Item item)
    {
        DialogueManager.Instance.EndDialogue();

        foreach (var pair in itemDialogues)
        {
            if (pair.item == item)
            {
                DialogueManager.Instance.StartDialogue(pair.dialogue);

                if (pair.consumesItem)
                    Inventory.Instance.RemoveItem(item);
                return;
            }
        }

        DialogueManager.Instance.StartDialogue(defaultDialogue);
    }


}
