using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public Camera playerCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);

        foreach (Collider hit in hits)
        {
            Item item = hit.GetComponent<Item>();

            if (item != null)
            {
                PickupItem(item);
                return;
            }
        }
    }

    void PickupItem(Item item)
    {
        Debug.Log("✅ Item ramassé : " + item.itemName);

        // AJOUT À L'INVENTAIRE (version propre)
        InventoryManager.Instance.AddToInventory(item);

        // Détruire l'objet ramassé
        Destroy(item.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
