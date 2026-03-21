using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public Sprite playerPortrait;
    public bool isBadDecision = false; 
    public DialogueLine[] lines;
}

[System.Serializable] //sauvegarde data
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker; 
    [TextArea(2, 5)]
    public string text;
}