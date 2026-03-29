using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterCard", menuName = "CardGame/CharacterCard")]
public class CharacterCardData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
    [TextArea(2, 4)]
    public string perkDescription;

    public int maxLife = 20;
    public int maxActionPoints = 3;
    public int actionPointsPerTurn = 3;

    public CardData[] startingDeck;
    public CardData rewardCard;
}