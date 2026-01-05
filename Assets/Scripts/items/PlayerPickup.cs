using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 5f; // On augmente un peu la portée
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
        if (playerCamera == null) return;

        // On crée un rayon qui part du centre de la caméra
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // SphereCast utilise un rayon plus large (0.2) pour faciliter le ramassage
        if (Physics.SphereCast(ray, 0.2f, out hit, pickupRange, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                InventoryManager.Instance.AddToInventory(item);
                Destroy(item.gameObject);
                Debug.Log("Objet ramassé !");
            }
        }
    }
}