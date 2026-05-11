using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public Sprite playerPortrait;
    public bool isBadDecision = false;
    public CardData rewardCard;
    public DialogueLine[] lines;

    public bool triggersCardGame = false;     
    public CharacterCardData enemyCardData;   
    public CharacterCardData playerCardData;   
    public Item[] cardGameRewards;
}

[System.Serializable] 
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker; 
    [TextArea(2, 5)]
    public string text;
}