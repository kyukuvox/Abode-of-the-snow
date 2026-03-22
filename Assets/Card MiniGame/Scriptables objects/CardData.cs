using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite cardSprite;
    [TextArea(2, 4)]
    public string description;

    public enum CardType { Attack, Defense, Recharge }
    public CardType cardType;

    public enum AttackType { None, HitDefense, HitRecharge, HitLife }
    public AttackType attackType; 

    public int actionCost;  
    public int power;        
    public int delayTurns;  
}