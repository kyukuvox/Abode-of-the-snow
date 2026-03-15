using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;    // Image du PNJ
    public Sprite playerPortrait; // Image du joueur

    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker; // Qui parle sur cette ligne ?
    [TextArea(2, 5)]
    public string text;
}