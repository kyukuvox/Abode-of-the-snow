using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData defaultDialogue;
    public float interactionRange = 2f;
    public GameObject interactionSprite;

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

        if (interactionSprite != null)
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
            else if (playerInRange && dialogueCooldown <= 0f)
            {
                dialogueCooldown = 1f;
                TriggerDialogue();
            }
        }
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