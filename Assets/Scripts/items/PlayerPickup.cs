using UnityEngine;
using System.Linq;

public class PlayerPickup_Complet : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public Camera playerCamera;
    public Transform flashlightHoldPoint; // FlashlightHolder sur la caméra

    [Header("Flashlight Offsets")]
    public Vector3 flashlightPositionOffset = new Vector3(0, -0.1f, 0.3f);
    public Vector3 flashlightRotationOffset = Vector3.zero;

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip flashlightOnSound;
    public AudioClip flashlightOffSound;
    private AudioSource audioSource;

    private Flashlight playerFlashlight; // lampe récupérée
    private PlayerObjectives playerObjectives;

    void Start()
    {
        playerObjectives = GetComponent<PlayerObjectives>();
        if (playerObjectives == null)
            Debug.LogWarning("⚠️ PlayerObjectives non trouvé sur le joueur !");

        // Initialiser l'AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (playerCamera == null)
            Debug.LogWarning("⚠️ PlayerCamera non assignée !");
        if (flashlightHoldPoint == null)
            Debug.LogWarning("⚠️ FlashlightHoldPoint non assigné !");
    }

    void Update()
    {
        // 🔹 Pickup items avec E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }

        // 🔹 Allumer/éteindre la lampe avec F
        if (playerFlashlight != null && Input.GetKeyDown(KeyCode.F))
        {
            playerFlashlight.ToggleFlashlight();

            if (audioSource != null)
            {
                if (playerFlashlight.IsOn)
                {
                    if (flashlightOnSound != null)
                        audioSource.PlayOneShot(flashlightOnSound);
                }
                else
                {
                    if (flashlightOffSound != null)
                        audioSource.PlayOneShot(flashlightOffSound);
                }
            }
        }
    }

    void TryPickupItem()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 1f);

        RaycastHit[] hits = Physics.SphereCastAll(ray, 0.6f, pickupRange, itemLayer);

        foreach (var h in hits.OrderBy(h => h.distance))
        {
            Item item = h.collider.GetComponentInParent<Item>();
            if (item == null) item = h.collider.GetComponentInChildren<Item>();

            if (item != null && item.canBePickedUp)
            {
                // Jouer le son de récupération
                if (pickupSound != null && audioSource != null)
                    audioSource.PlayOneShot(pickupSound);

                // 🔹 Si c'est une lampe
                Flashlight flashlight = item.GetComponent<Flashlight>();
                if (flashlight != null)
                {
                    if (flashlightHoldPoint == null)
                    {
                        Debug.LogWarning("FlashlightHoldPoint non assigné !");
                        return;
                    }

                    flashlight.transform.SetParent(flashlightHoldPoint);
                    flashlight.transform.localPosition = flashlightPositionOffset;
                    flashlight.transform.localRotation = Quaternion.Euler(flashlightRotationOffset);

                    playerFlashlight = flashlight;

                    // Désactiver le collider pour ne pas ramasser à nouveau
                    Collider col = flashlight.GetComponent<Collider>();
                    if (col != null) col.enabled = false;

                    // Optionnel : allumer la lampe dès le pickup
                    flashlight.ToggleFlashlight();

                    return;
                }
                else
                {
                    // 🔹 Ajouter à l'inventaire
                    InventoryManager.Instance.AddItem(item);

                    // 🔹 Mettre à jour PlayerObjectives
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
