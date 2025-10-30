using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
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
    }

    public void Interact()
    {
        isOpen = !isOpen; // change l'état

        // joue le son correspondant
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
