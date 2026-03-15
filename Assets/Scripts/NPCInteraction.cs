using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData defaultDialogue;   // Dialogue de base du PNJ
    public float interactionRange = 2f;    // Distance pour interagir

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Vérifie si le joueur est assez proche
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        // Appui sur E (ancien système d'input)
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
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