using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeRoomToMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu"; // Nom exact de la scène menu

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f; // s'assurer que le temps reprend
            SceneManager.LoadScene(menuSceneName); // charge la scène du menu
        }
    }
}
