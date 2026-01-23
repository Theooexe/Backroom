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
        // Slots vides au départ
        if (cleSlot != null) cleSlot.Hide();
        if (marteauSlot != null) marteauSlot.Hide();

        Debug.Log("InventoryManager initialisé !");
    }

    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("AddItem appelé avec item null");
            return;
        }

        if (item.icon == null)
        {
            Debug.LogWarning($"L'item {item.name} n'a pas d'icône assignée !");
            return;
        }

        switch (item.type)
        {
            case ItemType.Cle:
                if (cleSlot != null) cleSlot.SetItem(item.icon);
                else Debug.LogError("cleSlot non assigné dans InventoryManager !");
                break;

            case ItemType.Marteau:
                if (marteauSlot != null) marteauSlot.SetItem(item.icon);
                else Debug.LogError("marteauSlot non assigné dans InventoryManager !");
                break;

            default:
                Debug.LogWarning("Type d'item non géré : " + item.type);
                break;
        }

        Debug.Log($"Item ajouté à l'inventaire : {item.type}");
    }
}
