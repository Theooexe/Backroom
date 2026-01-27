using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Slots UI")]
    public ItemSlot cleSlot;
    public ItemSlot marteauSlot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

   private void Start()
    {
        if (cleSlot != null) cleSlot.Hide();
        if (marteauSlot != null) marteauSlot.Hide();
    }


    public void AddItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item.icon == null)
        {
            return;
        }

        switch (item.type)
        {
            case ItemType.Cle:
                if (cleSlot != null) cleSlot.SetItem(item.icon);
                break;

            case ItemType.Marteau:
                if (marteauSlot != null) marteauSlot.SetItem(item.icon);
                break;

            default:
                break;
        }
    }
}
