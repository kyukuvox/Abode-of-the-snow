using UnityEngine;
using System.Collections.Generic;

public class NPCWithItemDialogue : NPCInteraction
{
    [System.Serializable]
    public class ItemDialoguePair
    {
        public Item item;           // L'item qui déclenche ce dialogue
        public DialogueData dialogue; // Le dialogue correspondant
    }

    public List<ItemDialoguePair> itemDialogues; // Liste des paires item/dialogue

    //Appelé par ItemSlotUI quand on clique sur un slot
    public void ReceiveItem(Item item)
    {
        // Coupe proprement le dialogue en cours avant d'en lancer un nouveau
        DialogueManager.Instance.EndDialogue();

        foreach (var pair in itemDialogues)
        {
            if (pair.item == item)
            {
                DialogueManager.Instance.StartDialogue(pair.dialogue);
                return;
            }
        }

        DialogueManager.Instance.StartDialogue(defaultDialogue);
    }
  

}
