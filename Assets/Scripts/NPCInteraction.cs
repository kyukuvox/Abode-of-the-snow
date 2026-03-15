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
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Si un dialogue est en cours → on l'avance ou le ferme
            if (DialogueManager.Instance.IsActive())
            {
                DialogueManager.Instance.OnPressE();
            }
            // Sinon → on démarre le dialogue si le joueur est assez proche
            else if (playerInRange)
            {
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