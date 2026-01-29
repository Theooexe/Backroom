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
            return;
        }

        if (!inventory.HasHammer)
        {
            Debug.Log("Trouvez un outil pour casser la planche");
            return;
        }

      
        inventory.RemoveHammer();

        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);
        Destroy(gameObject, 0.05f);
    }
}
