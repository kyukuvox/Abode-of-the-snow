using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    //POUR DONNER SPRITE ET NOM ITEM

    public string itemName;
    public Sprite itemIcon;
    public Sprite descriptionSprite;

    [TextArea(2, 5)]
    public string description;

}
