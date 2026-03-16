using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    //POUR DONNER SPRITE ET NOM ITEM

    public string itemName;
    public Sprite itemIcon; 
   
}
