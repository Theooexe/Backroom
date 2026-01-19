using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f; // distance max d'interaction
    public Camera playerCamera;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // touche pour interagir
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // Vérifie si l'objet a un composant interactif
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
