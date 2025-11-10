using UnityEngine;
using UnityEngine.SceneManagement;   // pour LoadScene

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;              // Panel avec l'image + boutons

    [Header("Config")]
    public KeyCode pauseKey = KeyCode.Escape;  // touche pour ouvrir/fermer
    public string mainMenuSceneName = "MainScene";

    [Header("Gameplay")]
    public MonoBehaviour[] scriptsToDisableOnPause; // scripts de contrôle à couper

    public bool IsPaused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var s in scriptsToDisableOnPause)
        {
            if (s != null) s.enabled = false;
        }
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var s in scriptsToDisableOnPause)
        {
            if (s != null) s.enabled = true;
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
        