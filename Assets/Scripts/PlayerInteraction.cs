using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f; 
    public Camera playerCamera;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
