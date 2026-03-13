using UnityEngine;

public class NPC : MonoBehaviour
{
   
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            NPCInteraction.Instance.SetCurrentNPC(this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            NPCInteraction.Instance.ClearCurrentNPC();
    }
}
