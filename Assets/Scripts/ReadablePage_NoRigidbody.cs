using UnityEngine;

public class ReadablePage_NoRigidbody : MonoBehaviour
{
    public DocumentPage documentPage;   // Assigner DocumentManager ici
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 3f;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
            Debug.LogError("Camera principale non trouvée ! Tag MainCamera ?");
        if (documentPage == null)
            Debug.LogWarning("DocumentPage non assigné !");
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (documentPage == null || playerCamera == null) return;

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Collider pageCollider = GetComponent<Collider>();
                if (pageCollider != null && hit.collider == pageCollider)
                {
                    documentPage.Open();
                }
                else if(pageCollider == null)
                {
                    Debug.LogWarning("Collider manquant sur la page 3D !");
                }
            }
        }
    }
}
