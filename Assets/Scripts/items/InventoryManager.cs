using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
<<<<<<< Updated upstream

    // On utilise directement tes scripts ItemSlot
    public ItemSlot slotCle;
    public ItemSlot slotMarteau;

    private void Awake()
    {
        Instance = this;
        
        // On vide les slots proprement au début
        if(slotCle != null) slotCle.ClearSlot();
        if(slotMarteau != null) slotMarteau.ClearSlot();
=======
    
    [Header("Icônes des items")]
    public GameObject iconCle;
    public GameObject iconMarteau;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
>>>>>>> Stashed changes
    }
    
    void Start()
    {
<<<<<<< Updated upstream
        // On demande au slot de s'afficher lui-même
        switch (item.type)
        {
            case ItemType.Cle:
                slotCle.SetSprite(item.icon);
                break;
            case ItemType.Marteau:
                slotMarteau.SetSprite(item.icon);
=======
        // Cacher les icônes au départ
        if (iconCle != null) iconCle.SetActive(false);
        if (iconMarteau != null) iconMarteau.SetActive(false);
        
        Debug.Log("InventoryManager initialisé !");
    }
    
    public void AddItem(string itemName)
    {
        Debug.Log("Tentative d'ajout de l'item: " + itemName);
        
        switch(itemName.ToLower())
        {
            case "cle":
            case "key":
                if (iconCle != null)
                {
                    iconCle.SetActive(true);
                    Debug.Log("✓ Clé affichée dans l'inventaire!");
                }
                else
                {
                    Debug.LogError("IconCle n'est pas assigné!");
                }
                break;
                
            case "marteau":
            case "hammer":
                if (iconMarteau != null)
                {
                    iconMarteau.SetActive(true);
                    Debug.Log("✓ Marteau affiché dans l'inventaire!");
                }
                else
                {
                    Debug.LogError("IconMarteau n'est pas assigné!");
                }
                break;
                
            default:
                Debug.LogWarning("Item inconnu: " + itemName);
>>>>>>> Stashed changes
                break;
        }
    }
}