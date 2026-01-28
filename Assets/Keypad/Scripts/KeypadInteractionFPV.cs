using UnityEngine;

public class KeypadInteractionFPV : MonoBehaviour, IInteractable
{
    [SerializeField] private Canvas keypadCanvas;
    public void Interact()
    {
    
        if (keypadCanvas != null)
            keypadCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseKeypad()
    {
        if (keypadCanvas != null)
            keypadCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
