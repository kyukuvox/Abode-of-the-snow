using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    [System.Serializable]
    public class RequiredItemsCondition
    {
        public Item triggerItem;
        public Item[] requiredItems;
        public DialogueData missingItemsDialogue;
    }

    public List<ItemDialoguePair> itemDialogues;
    public DialogueData alreadyDefeatedDialogue;
    public DialogueData fightDialogue;
    public RequiredItemsCondition itemsCondition;

    private bool hasBeenDefeated = false;
    private bool hasBeenFought = false;
    private List<Item> consumedItems = new List<Item>();
    private HoverParticleManager hoverParticles;

    protected new void Start()
    {
        base.Start();
        hoverParticles = GetComponent<HoverParticleManager>();
    }

    public void SetFought()
    {
        hasBeenFought = true;
    }

    void OnMouseEnter()
    {
        if (interactionType != InteractionType.MouseClick) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;

        if (interactionSprite != null)
            interactionSprite.SetActive(true);
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (interactionType != InteractionType.MouseClick) return;

        if (interactionSprite != null)
            interactionSprite.SetActive(false);
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    public void SetDefeated(bool defeated)
    {
        hasBeenDefeated = defeated;
    }

    public bool HasBeenDefeated() { return hasBeenDefeated; }

    bool HasAllRequiredItems()
    {
        if (itemsCondition == null || itemsCondition.requiredItems == null ||
            itemsCondition.requiredItems.Length == 0)
            return true;

        foreach (Item requiredItem in itemsCondition.requiredItems)
        {
            if (!Inventory.Instance.items.Contains(requiredItem))
                return false;
        }
        return true;
    }

    public override void TriggerDialogue()
    {
        dialogueCooldown = 1f;

        if (hasBeenDefeated)
        {
            if (alreadyDefeatedDialogue != null)
                DialogueManager.Instance.StartDialogue(alreadyDefeatedDialogue, this);
            else
                DialogueManager.Instance.StartDialogue(defaultDialogue, this);
            return;
        }

        if (hasBeenFought && fightDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(fightDialogue, this);
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
                if (pair.consumesItem &&
                    itemsCondition != null &&
                    itemsCondition.triggerItem == item &&
                    itemsCondition.requiredItems != null &&
                    itemsCondition.requiredItems.Length > 0)
                {
                    if (!HasAllRequiredItems())
                    {
                        Inventory.Instance.AddItem(item);
                        if (itemsCondition.missingItemsDialogue != null)
                            DialogueManager.Instance.StartDialogue(itemsCondition.missingItemsDialogue, this);
                        else
                            DialogueManager.Instance.StartDialogue(defaultDialogue, this);
                        return;
                    }
                }

                if (!pair.consumesItem)
                    Inventory.Instance.AddItem(item);
                else
                {
                    if (!consumedItems.Contains(item))
                        consumedItems.Add(item);

                    if (Inventory.Instance.onItemChangedCallback != null)
                        Inventory.Instance.onItemChangedCallback.Invoke();
                }

                if (pair.activatesPortal)
                {
                    PortalAnimator[] allPortals = FindObjectsByType<PortalAnimator>(FindObjectsSortMode.None);
                    foreach (PortalAnimator portal in allPortals)
                        portal.TryActivate(pair.dialogue);
                }

                DialogueManager.Instance.StartDialogue(pair.dialogue, this);

                if (AllConsumedItemsGiven() && fightDialogue != null)
                    StartCoroutine(LaunchFightDialogueAfterDelay());

                return;
            }
        }

        Inventory.Instance.AddItem(item);
        DialogueManager.Instance.StartDialogue(defaultDialogue, this);
    }

    IEnumerator LaunchFightDialogueAfterDelay()
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