using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Slots UI")]
    public ItemSlot cleSlot;
    public ItemSlot marteauSlot;

    public bool HasKey { get; private set; }
    public bool HasHammer { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (cleSlot != null) cleSlot.Hide();
        if (marteauSlot != null) marteauSlot.Hide();

        HasKey = false;
        HasHammer = false;
    }

    public void AddItem(Item item)
    {
        if (item == null || item.icon == null) return;

        switch (item.type)
        {
            case ItemType.Cle:
                HasKey = true;
                if (cleSlot != null)
                    cleSlot.SetItem(item.icon);
                break;

            case ItemType.Marteau:
                HasHammer = true;
                if (marteauSlot != null)
                    marteauSlot.SetItem(item.icon);
                break;
        }
    }

    public void RemoveHammer()
    {
        HasHammer = false;

        if (marteauSlot != null)
            marteauSlot.Hide();
    }

    public void RemoveKey()
    {
        HasKey = false;

        if (cleSlot != null)
            cleSlot.Hide();
    }
}
