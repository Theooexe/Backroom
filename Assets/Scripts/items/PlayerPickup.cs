using UnityEngine;
using System.Linq;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public LayerMask itemLayer;     // (optionnel ici, mais on le garde si tu veux l'utiliser ensuite)
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

    Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 1f);

    RaycastHit[] hits = Physics.SphereCastAll(ray, 0.6f, pickupRange);

    foreach (var h in hits.OrderBy(h => h.distance))
    {
        Item item = h.collider.GetComponentInParent<Item>();
        if (item == null) item = h.collider.GetComponentInChildren<Item>();

        if (item != null && item.canBePickedUp)
        {
            InventoryManager.Instance.AddItem(item);
            Destroy(item.gameObject);
            Debug.Log("Objet ramassé : " + item.type);
            return;
        }
    }

    Debug.Log("Touché, mais aucun Item trouvé (sol/mur uniquement).");
}

}
