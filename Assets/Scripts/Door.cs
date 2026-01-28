using UnityEngine;
using TMPro;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isExitDoor = false;                  
    public PlayerObjectives playerObjectives;        

    [Header("Blocking Object (optional)")]
    public GameObject blockingBoard;                 

    [Header("Door Settings")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public float openAngle = 90f;
    public float smooth = 2f;

    [Header("UI Messages")]
    public TMP_Text messageText;                     
    public float messageDuration = 2f;              
    private float messageTimer = 0f;

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

    void Update()
    {
        // Animation ouverture / fermeture
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              isOpen ? openRotation : closedRotation,
                                              Time.deltaTime * smooth);

        // Timer pour message
        if (messageText != null && messageText.gameObject.activeSelf)
        {
            messageTimer += Time.unscaledDeltaTime;
            if (messageTimer >= messageDuration)
            {
                messageText.gameObject.SetActive(false);
                messageTimer = 0f;
            }
        }
    }

    public void Interact()
    {
        if (isExitDoor)
        {
            if (playerObjectives == null)
                return;

            // Message uniquement si la clé manque
            if (!playerObjectives.hasKey)
            {
                ShowMessage("Détruisez la planche et trouvez la clé");
                return;
            }

            // Si planche encore là, la porte ne s'ouvre pas mais aucun message
            if (blockingBoard != null)
                return;
        }

        // Ouvrir / fermer la porte
        isOpen = !isOpen;

        if (audioSource != null)
        {
            if (isOpen && openSound != null)
                audioSource.PlayOneShot(openSound);
            else if (!isOpen && closeSound != null)
                audioSource.PlayOneShot(closeSound);
        }
    }

    public void RemoveBlockingBoard()
    {
        if (blockingBoard != null)
        {
            Destroy(blockingBoard);
            blockingBoard = null;
        }
    }

    private void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
            messageText.gameObject.SetActive(true);
            messageTimer = 0f;
        }
        else
        {
            Debug.Log(text);
        }
    }
}
