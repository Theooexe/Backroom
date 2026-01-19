using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public Camera playerCamera;

    [Header("Objectives")]
    [SerializeField] private PlayerObjectives playerObjectives;

    [Header("Item Names (must match Item.itemName)")]
    [SerializeField] private string keyItemName = "Cle";
    [SerializeField] private string hammerItemName = "Marteau";

    private void Awake()
    {
        // Auto-find si pas assigné dans l'inspector
        if (playerObjectives == null)
            playerObjectives = GetComponent<PlayerObjectives>();

        if (playerObjectives == null)
            playerObjectives = FindObjectOfType<PlayerObjectives>();

    }

    void Update()
    {
        // Appuyer sur E pour ramasser
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Touche E pressée !");
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        // Chercher tous les objets avec le Layer "Item" dans le rayon
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);

        Debug.Log("Nombre d'objets détectés : " + hits.Length);

        foreach (Collider hit in hits)
        {
            Debug.Log("Objet trouvé : " + hit.gameObject.name);

            // Vérifier si l'objet a le script Item
            Item item = hit.GetComponent<Item>();

            if (item != null)
            {
                PickupItem(item);
                return; // Ramasser un seul objet à la fois
            }
            else
            {
                Debug.Log("Pas de script Item sur cet objet !");
            }
        }
    }

    void PickupItem(Item item)
    {

        Debug.Log("✅ Item ramassé : " + item.itemName);
        string picked = item.itemName.Trim();

        // Mettre à jour les objectifs selon l'item ramassé
        if (playerObjectives != null)
        {
            if (picked == keyItemName.Trim())
            {
                playerObjectives.hasKey = true;
                Debug.Log("🔑 Clé obtenue !");
            }
            else if (picked == hammerItemName.Trim())
            {
                playerObjectives.hasHammer = true;
                Debug.Log("🔨 Marteau obtenu !");
            }
        }

        // Détruire l'objet
        Destroy(item.gameObject);

    }

    // Dessiner la zone de ramassage dans la Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
