using UnityEngine;

public class EscapeRoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject finishCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (finishCanvas != null)
            {
                finishCanvas.SetActive(true); // afficher le Canvas
            }
            Time.timeScale = 0f; // pause le jeu
        }
    }
}
