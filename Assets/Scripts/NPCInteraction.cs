using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData defaultDialogue;   // Dialogue de base du PNJ
    public float interactionRange = 2f;    // Distance pour interagir

    private Transform player;
    private bool playerInRange = false;
    private float dialogueCooldown = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        // Diminue le cooldown avec le temps
        if (dialogueCooldown > 0f)
            dialogueCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance.IsActive())
            {
                DialogueManager.Instance.OnPressE();
            }
            else if (playerInRange && dialogueCooldown <= 0f)
            {
                dialogueCooldown = 1f; // ← remet le compteur à 1 seconde
                TriggerDialogue();
            }
        }
    }

    public virtual void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(defaultDialogue);
    }

    // Visualise la portée dans l'éditeur (cercle vert)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}