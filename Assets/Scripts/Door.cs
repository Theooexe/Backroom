using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    [Header("Exit Door Lock (optional)")]
    public bool isExitDoor = false;
    public PlayerObjectives playerObjectives;
    public bool requireKey = true;
    public bool requireHammer = true;
    public bool requireCode = true;

    public AudioClip openSound;   // Son ouverture
    public AudioClip closeSound;  // Son fermeture
    public float openAngle = 90f; // Angle d'ouverture
    public float smooth = 2f;     // Vitesse d'ouverture

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
            playerObjectives = FindObjectOfType<PlayerObjectives>();

    }

    public void Interact()
    {
        // Si c'est la porte de sortie, on vérifie les conditions
        if (isExitDoor)
        {
            if (playerObjectives == null)
            {
                Debug.LogWarning("⚠️ Exit door: PlayerObjectives manquant");
                return;
            }

            bool ok =
                (!requireKey || playerObjectives.hasKey) &&
                (!requireHammer || playerObjectives.hasHammer) &&
                (!requireCode || playerObjectives.codeOk);

            if (!ok)
            {
                Debug.Log("🔒 Porte verrouillée: il manque un/des élément(s)");
                Debug.Log($"🔒 Etat objectifs: key={playerObjectives.hasKey} hammer={playerObjectives.hasHammer} code={playerObjectives.codeOk}");
                return; // on n'ouvre pas
            }
        }

        // Sinon fonctionnement normal
        isOpen = !isOpen;

        if (audioSource != null)
        {
            if (isOpen && openSound != null)
                audioSource.PlayOneShot(openSound);
            else if (!isOpen && closeSound != null)
                audioSource.PlayOneShot(closeSound);
        }
    }


    void Update()
    {
        if (isOpen)
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * smooth);
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * smooth);
    }
}