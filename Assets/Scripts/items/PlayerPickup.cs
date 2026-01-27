using UnityEngine;
using System.Linq;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public Camera playerCamera;
    public Transform flashlightHoldPoint; // position où la lampe sera attachée

    private Flashlight playerFlashlight; // lampe récupérée

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }

        // Allumer/éteindre la lampe si le joueur a récupéré une Flashlight
        if (playerFlashlight != null && Input.GetKeyDown(KeyCode.F))
        {
            playerFlashlight.ToggleFlashlight();
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
                // Vérifier si c'est une Flashlight
                Flashlight flashlight = item.GetComponent<Flashlight>();
                if (flashlight != null)
                {
                    // Attacher la lampe au joueur à la position holdPoint
                    flashlight.transform.SetParent(flashlightHoldPoint);
                    flashlight.transform.localPosition = Vector3.zero;
                    flashlight.transform.localRotation = Quaternion.identity;

                    // Stocker la lampe pour pouvoir l'utiliser
                    playerFlashlight = flashlight;

                    // Désactiver le collider pour éviter de la ramasser à nouveau
                    Collider col = flashlight.GetComponent<Collider>();
                    if (col != null) col.enabled = false;

                    // On ne passe pas par l'inventaire et on ne détruit pas la lampe
                    return;
                }
                else
                {
                    // Pour les autres items, garder l'ancien comportement
                    InventoryManager.Instance.AddItem(item);
                    Destroy(item.gameObject);
                    return;
                }
            }
        }
    }
}
