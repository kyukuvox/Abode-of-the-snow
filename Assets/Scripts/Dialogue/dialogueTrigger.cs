using UnityEngine;

public class dialogueTrigger : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] 
    private GameObject VisualCue;

    private bool playerInRange;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Awake()
    {
        playerInRange = false;
        VisualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !dialogueManager.GetInstance().dialogueIsPlaying)
        {
            VisualCue.SetActive(true);
            if (InputManager.GetInstance().GetInteractPressed())
            {
                dialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            VisualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false ;
        }
    }
}
