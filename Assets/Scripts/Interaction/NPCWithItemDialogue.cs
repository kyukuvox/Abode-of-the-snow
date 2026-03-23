using UnityEngine;
using System.Collections.Generic;

public class NPCWithItemDialogue : NPCInteraction
{
    [System.Serializable]
    public class ItemDialoguePair
    {
        public Item item;
        public DialogueData dialogue;
        public bool consumesItem = true;
    }

    public List<ItemDialoguePair> itemDialogues;
    public DialogueData alreadyDefeatedDialogue;
    private bool hasBeenDefeated = false;

    public void SetDefeated(bool defeated)
    {
        hasBeenDefeated = defeated;
    }

    public bool HasBeenDefeated() { return hasBeenDefeated; }

    public override void TriggerDialogue()
    {
        if (hasBeenDefeated && alreadyDefeatedDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(alreadyDefeatedDialogue, this);
            return;
        }

        DialogueManager.Instance.StartDialogue(defaultDialogue, this);
    }

    public void ReceiveItem(Item item)
    {
        DialogueManager.Instance.EndDialogue();
        dialogueCooldown = 1f;

        foreach (var pair in itemDialogues)
        {
            if (pair.item == item)
            {
                DialogueManager.Instance.StartDialogue(pair.dialogue, this);
                if (pair.consumesItem)
                    Inventory.Instance.RemoveItem(item);
                return;
            }
        }

        DialogueManager.Instance.StartDialogue(defaultDialogue, this);
    }
}