using UnityEngine;

public class DocumentPage : MonoBehaviour
{
    public GameObject documentCanvas;

    public void Open()
    {
        documentCanvas.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        documentCanvas.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
