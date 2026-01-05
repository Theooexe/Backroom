using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Slots")]
    public Image slotClavier;
    public Image slotCle;
    public Image slotMarteau;

    private void Awake()
    {
        Instance = this;
    }

    public void AddToInventory(Item item)
    {
        Sprite icon = item.icon; // icône venant du prefab

        switch (item.type)
        {
            case ItemType.Clavier:
                slotClavier.sprite = icon;
                slotClavier.color = Color.white;
                break;

            case ItemType.Cle:
                slotCle.sprite = icon;
                slotCle.color = Color.white;
                break;

            case ItemType.Marteau:
                slotMarteau.sprite = icon;
                slotMarteau.color = Color.white;
                break;
        }
    }
}
