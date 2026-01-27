using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;

    [Header("Config")]
    public string mainMenuSceneName = "MainScene";

    [Header("Gameplay")]
    public MonoBehaviour[] scriptsToDisableOnDeath;

    private PauseMenu pauseMenu;

    void Start()
    {
        // sécurité : au lancement, on cache le panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        pauseMenu = Object.FindFirstObjectByType<PauseMenu>();
    }

    public void ShowGameOver()
    {
        if (pauseMenu != null)
        {
            pauseMenu.allowPause = false;           
            if (pauseMenu.IsPaused)
            {
                // on ferme son panel au cas où
                pauseMenu.Resume();
            }
        }
        
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var s in scriptsToDisableOnDeath)
        {
            if (s != null) s.enabled = false;
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        var current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
