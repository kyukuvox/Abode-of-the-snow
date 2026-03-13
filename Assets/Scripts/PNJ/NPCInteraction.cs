using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public static NPCInteraction Instance;
    private NPC _currentNPC = null;

    void Awake() => Instance = this;

    // Appelé par NPC.cs quand le joueur entre/sort de la zone
    public void SetCurrentNPC(NPC npc) => _currentNPC = npc;
    public void ClearCurrentNPC() => _currentNPC = null;

    // Appelé au clic sur un slot d'inventaire
    public void TryPresentItem(string itemId)
    {
        if (_currentNPC == null)
        {
            Debug.Log("Aucun PNJ à portée !");
            return;
        }
        DialogueManager.Instance.StartDialogue(itemId);
    }
}