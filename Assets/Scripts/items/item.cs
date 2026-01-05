using UnityEngine;

public enum ItemType { Clavier, Lampe, Cle, Marteau }

public class Item : MonoBehaviour
{
    public ItemType type;
    public string itemName;
    public bool canBePickedUp = true;

    [Header("UI Icon")]
    public Sprite icon;
}
