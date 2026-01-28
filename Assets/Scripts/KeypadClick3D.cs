using UnityEngine;
using NavKeypad; 

public class KeypadClick3D : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("KeypadButton"))
                {
                    string buttonValue = hit.collider.name;

                    // Récupère le script Keypad dans le parent
                    Keypad keypad = hit.collider.GetComponentInParent<Keypad>();
                    if (keypad != null)
                        keypad.AddInput(buttonValue);
                }
            }
        }
    }
}
