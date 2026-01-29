using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject hud;
    public AudioSource chaseMusic;
    public string mainMenuSceneName = "MainScene";
    public MonoBehaviour[] scriptsToDisableOnDeath;

    private PauseMenu pauseMenu;

    void Start()
    {
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
                pauseMenu.Resume();
        }

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var s in scriptsToDisableOnDeath)
            if (s != null) s.enabled = false;

        if (chaseMusic != null && chaseMusic.isPlaying)
            chaseMusic.Stop();

        if (hud != null)
            hud.SetActive(false);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
