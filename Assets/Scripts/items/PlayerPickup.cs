using UnityEngine;
using System.Linq;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public Camera playerCamera;
    public Transform flashlightHoldPoint; // position où la lampe sera attachée

    private Flashlight playerFlashlight; // lampe récupérée
    private PlayerObjectives playerObjectives;

    void Start()
    {
        playerObjectives = GetComponent<PlayerObjectives>();
        if (playerObjectives == null)
            Debug.LogWarning("⚠️ PlayerObjectives non trouvé sur le joueur !");
    }

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
                // 🔹 Si c'est une lampe
                Flashlight flashlight = item.GetComponent<Flashlight>();
                if (flashlight != null)
                {
                    // Attacher la lampe au joueur
                    flashlight.transform.SetParent(flashlightHoldPoint);
                    flashlight.transform.localPosition = Vector3.zero;
                    flashlight.transform.localRotation = Quaternion.identity;

                    playerFlashlight = flashlight;

                    // Désactiver le collider pour ne pas ramasser à nouveau
                    Collider col = flashlight.GetComponent<Collider>();
                    if (col != null) col.enabled = false;

                    return;
                }
                else
                {
                    // 🔹 Ajouter à l'inventaire
                    InventoryManager.Instance.AddItem(item);

                    // 🔹 Mettre à jour PlayerObjectives pour la porte
                    if (playerObjectives != null)
                    {
                        switch (item.type)
                        {
                            case ItemType.Cle:
                                playerObjectives.hasKey = true;
                                Debug.Log("🔑 Clé récupérée (PlayerObjectives)");
                                break;
                            case ItemType.Marteau:
                                playerObjectives.hasHammer = true;
                                Debug.Log("🔨 Marteau récupéré (PlayerObjectives)");
                                break;
                        }
                    }

                    // Détruire l'item dans le monde
                    Destroy(item.gameObject);
                    return;
                }
            }
        }
    }
}
