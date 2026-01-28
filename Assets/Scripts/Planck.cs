using UnityEngine;

public class Plank : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip breakSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Interact()
    {
        InventoryManager inventory = InventoryManager.Instance;

        if (inventory == null)
        {
            Debug.LogWarning("⚠️ InventoryManager introuvable");
            return;
        }

        if (!inventory.HasHammer)
        {
            Debug.Log("🛑 Il faut un marteau pour casser la planche");
            return;
        }

        // 🔨 Consommer le marteau
        inventory.RemoveHammer();

        // 🔊 Son
        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);

        // 🪵 Détruire la planche
        Destroy(gameObject, 0.05f);
    }
}
