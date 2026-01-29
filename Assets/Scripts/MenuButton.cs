using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SceneMenu"; // Nom exact de la scène menu

    public void GoToMenu()
    {
        // Assure que le temps est normal avant de charger la scène
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }
}
