using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData defaultDialogue;
    public float interactionRange = 2f;
    public GameObject interactionSprite;

    [Header("Son")]
    public AudioClip interactionSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    public enum InteractionType { KeyPress, MouseClick }
    public InteractionType interactionType = InteractionType.KeyPress;

    protected Transform player;
    protected bool playerInRange = false;
    protected float dialogueCooldown = 0f;
    private bool wasDialogueActive = false;
    private HoverParticleManager hoverParticles;

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hoverParticles = GetComponent<HoverParticleManager>();

        if (interactionSprite != null)
            interactionSprite.SetActive(false);
    }

    void Update()
    {
        if (BadDecisionManager.Instance.isGameOver) return;
        if (BadDecisionManager.Instance.isOverlayActive) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
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
                    PlayInteractionSound();
                    dialogueCooldown = 1f;
                    TriggerDialogue();
                }
            }
        }

        if (GameStateManager.Instance.IsCinematicMode()) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (interactionType == InteractionType.KeyPress)
        {
            if (interactionSprite != null)
                interactionSprite.SetActive(playerInRange);

            if (hoverParticles != null)
            {
                if (playerInRange)
                    hoverParticles.Show();
                else
                    hoverParticles.Hide();
            }
        }

        if (dialogueCooldown > 0f)
            dialogueCooldown -= Time.deltaTime;

        bool isDialogueCurrentlyActive = DialogueManager.Instance.IsActive();
        if (wasDialogueActive && !isDialogueCurrentlyActive)
            dialogueCooldown = 1f;
        wasDialogueActive = isDialogueCurrentlyActive;
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

    void OnMouseDown()
    {
        if (interactionType != InteractionType.MouseClick) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;
        if (dialogueCooldown > 0f) return;

        PlayInteractionSound();
        dialogueCooldown = 1f;
        TriggerDialogue();
    }

    public void PlayInteractionSound()
    {
        SoundSettings.PlaySound(interactionSound, soundVolume, this);
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