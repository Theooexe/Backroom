using UnityEngine;

public class Plank : MonoBehaviour, IInteractable
{
    [SerializeField] private Door linkedDoor;
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
        PlayerObjectives objectives = FindFirstObjectByType<PlayerObjectives>();

        if (objectives == null)
        {
            Debug.LogWarning("⚠️ PlayerObjectives introuvable");
            return;
        }

        if (!objectives.hasHammer)
        {
            Debug.Log("🛑 Il faut un marteau pour casser la planche");
            return;
        }
        
        if (linkedDoor != null)
        {
            linkedDoor.RemoveHammerRequirement();
        }
        
        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);
        
        Destroy(gameObject, 0.05f);
    }
}