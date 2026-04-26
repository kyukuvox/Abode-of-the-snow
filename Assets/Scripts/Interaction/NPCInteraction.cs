using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData defaultDialogue;
    public float interactionRange = 2f;
    public GameObject interactionSprite;

    public enum InteractionType { KeyPress, MouseClick }
    public InteractionType interactionType = InteractionType.KeyPress;

    protected Transform player;
    protected bool playerInRange = false;
    protected float dialogueCooldown = 0f;
    private bool wasDialogueActive = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (interactionSprite != null)
            interactionSprite.SetActive(false);
    }

    void Update()
    {
        if (BadDecisionManager.Instance.isGameOver) return;
        if (BadDecisionManager.Instance.isOverlayActive) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (interactionType == InteractionType.KeyPress && interactionSprite != null)
            interactionSprite.SetActive(playerInRange);

        if (dialogueCooldown > 0f)
            dialogueCooldown -= Time.deltaTime;

        bool isDialogueCurrentlyActive = DialogueManager.Instance.IsActive();
        if (wasDialogueActive && !isDialogueCurrentlyActive)
            dialogueCooldown = 1f;
        wasDialogueActive = isDialogueCurrentlyActive;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ItemDescriptionManager.Instance.IsActive())
            {
                ItemDescriptionManager.Instance.ClosePanel();
                return;
            }

            if (DialogueManager.Instance.IsActive())
            {
                if (DialogueManager.Instance.IsWaitingForInput())
                    DialogueManager.Instance.OnPressE();
                return;
            }

            if (interactionType == InteractionType.KeyPress)
            {
                if (playerInRange && dialogueCooldown <= 0f)
                {
                    dialogueCooldown = 1f;
                    TriggerDialogue();
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (interactionType != InteractionType.MouseClick) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;

        if (interactionSprite != null)
            interactionSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (interactionType != InteractionType.MouseClick) return;

        if (interactionSprite != null)
            interactionSprite.SetActive(false);
    }

    void OnMouseDown()
    {
        if (interactionType != InteractionType.MouseClick) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (dialogueCooldown > 0f) return;

        dialogueCooldown = 1f;
        TriggerDialogue();
    }

    public virtual void TriggerDialogue()
    {
        dialogueCooldown = 1f;
        DialogueManager.Instance.StartDialogue(defaultDialogue, null);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}