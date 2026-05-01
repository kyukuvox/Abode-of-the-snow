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
        public bool activatesPortal = false;
    }

    public List<ItemDialoguePair> itemDialogues;
    public DialogueData alreadyDefeatedDialogue;
    public DialogueData fightDialogue; 

    private bool hasBeenDefeated = false;
    private List<Item> consumedItems = new List<Item>();

    public void SetDefeated(bool defeated)
    {
        hasBeenDefeated = defeated;
    }

    public bool HasBeenDefeated() { return hasBeenDefeated; }

    public override void TriggerDialogue()
    {
        dialogueCooldown = 1f;

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

        if (hasBeenDefeated)
        {
            foreach (var pair in itemDialogues)
            {
                if (pair.item == item && !pair.consumesItem)
                {
                    Inventory.Instance.AddItem(item);
                    DialogueManager.Instance.StartDialogue(pair.dialogue, this);
                    return;
                }
            }

            Inventory.Instance.AddItem(item);
            if (alreadyDefeatedDialogue != null)
                DialogueManager.Instance.StartDialogue(alreadyDefeatedDialogue, this);
            else
                DialogueManager.Instance.StartDialogue(defaultDialogue, this);
            return;
        }

        foreach (var pair in itemDialogues)
        {
            if (pair.item == item)
            {
                if (!pair.consumesItem)
                {
                    Inventory.Instance.AddItem(item);
                }
                else
                {
                    if (!consumedItems.Contains(item))
                        consumedItems.Add(item);

                    if (Inventory.Instance.onItemChangedCallback != null)
                        Inventory.Instance.onItemChangedCallback.Invoke();
                }

                if (pair.activatesPortal && PortalAnimator.Instance != null)
                    PortalAnimator.Instance.TryActivate(pair.dialogue);

                DialogueManager.Instance.StartDialogue(pair.dialogue, this);

                if (AllConsumedItemsGiven() && fightDialogue != null)
                    StartCoroutine(LaunchFightDialogueAfterDelay());

                return;
            }
        }

        Inventory.Instance.AddItem(item);
        DialogueManager.Instance.StartDialogue(defaultDialogue, this);
    }

    System.Collections.IEnumerator LaunchFightDialogueAfterDelay()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.IsActive());
        DialogueManager.Instance.StartDialogue(fightDialogue, this);
    }

    bool AllConsumedItemsGiven()
    {
        foreach (var pair in itemDialogues)
            if (pair.consumesItem && !consumedItems.Contains(pair.item))
                return false;
        return true;
    }
}