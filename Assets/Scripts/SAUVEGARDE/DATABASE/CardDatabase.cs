using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "CardGame/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public CardData[] allCards;        
    public CardData[] rewardCards;   
}