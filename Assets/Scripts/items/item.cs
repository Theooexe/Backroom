using UnityEngine;

public enum ItemType
{
    Cle = 0,
    Marteau = 1 ,
    Autre = 2
}

public class Item : MonoBehaviour
{
    public ItemType type;
    public string itemName;
    public bool canBePickedUp = true;

    [Header("UI Icon")]
    public Sprite icon;
}
