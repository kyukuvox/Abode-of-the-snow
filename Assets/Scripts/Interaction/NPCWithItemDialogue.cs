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

    public List<ItemDialoguePair> itemDialogues;
    public DialogueData alreadyDefeatedDialogue;
    public DialogueData fightDialogue;

    private bool hasBeenDefeated = false;
    private List<Item> consumedItems = new List<Item>();
    private HoverParticleManager hoverParticles;

    protected new void Start()
    {
        base.Start();
        hoverParticles = GetComponent<HoverParticleManager>();
    }

    void OnMouseEnter()
    {
        if (interactionType != InteractionType.MouseClick) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;

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
                    Inventory.Instance.AddItem(item);
                else
                {
                    if (!consumedItems.Contains(item))
                        consumedItems.Add(item);

                    if (Inventory.Instance.onItemChangedCallback != null)
                        Inventory.Instance.onItemChangedCallback.Invoke();
                }

                // Cherche tous les portails et essaie de les activer
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