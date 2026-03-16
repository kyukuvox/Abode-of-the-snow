using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    // POUR DATA DIALOGUES ET SPRITES !!

    public string npcName;
    public Sprite npcPortrait;    
    public Sprite playerPortrait; 

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