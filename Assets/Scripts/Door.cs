using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Exit Door Lock (optional)")]
    public bool isExitDoor = false;
    public PlayerObjectives playerObjectives;

    public bool requireKey = true;
    public bool requireCode = true;

    [Header("Door Settings")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public float openAngle = 90f;
    public float smooth = 2f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private AudioSource audioSource;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (isExitDoor && playerObjectives == null)
            playerObjectives = FindFirstObjectByType<PlayerObjectives>();
    }

    public void Interact()
    {
        // 🔒 Vérification porte de sortie
        if (isExitDoor)
        {
            if (playerObjectives == null)
            {
                Debug.LogWarning("⚠️ Exit door: PlayerObjectives manquant");
                return;
            }

            bool ok =
                (!requireKey || playerObjectives.hasKey) &&
                (!requireCode || playerObjectives.codeOk);

            if (!ok)
            {
                Debug.Log("🔒 Porte verrouillée");
                Debug.Log($"🔒 Etat: key={playerObjectives.hasKey} code={playerObjectives.codeOk}");
                return;
            }
        }

        // 🚪 Ouvrir / fermer
        isOpen = !isOpen;

        if (audioSource != null)
        {
            if (isOpen && openSound != null)
                audioSource.PlayOneShot(openSound);
            else if (!isOpen && closeSound != null)
                audioSource.PlayOneShot(closeSound);
        }
    }

    public void UnlockByCode()
    {
        requireCode = false;
        Debug.Log("🔓 Door: code validé");
    }


    void Update()
    {
        if (isOpen)
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * smooth);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * smooth);
    }
}
