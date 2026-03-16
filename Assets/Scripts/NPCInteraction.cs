using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public DialogueData defaultDialogue;   
    public float interactionRange = 2f;    

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
                dialogueCooldown = 1f;  // cooldown pour éviter spam dialogue 
                TriggerDialogue();
            }
        }
    }

    public virtual void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(defaultDialogue);
    }

  
    void OnDrawGizmosSelected() // Agit comme un collider 2d (mieux)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}