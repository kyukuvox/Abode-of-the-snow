using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public Sprite playerPortrait;
    public bool isBadDecision;
    public bool triggersCardGame;
    public CharacterCardData enemyCardData;
    public CharacterCardData playerCardData;
    public Item[] cardGameRewards;
    public CardData rewardCard;
    public bool activatesPortalAfterCardGame; 
    public DialogueLine[] lines;
}

[System.Serializable] 
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker; 
    [TextArea(2, 5)]
    public string text;
}