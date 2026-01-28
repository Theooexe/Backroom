using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public GameObject HUD;

    [Header("Player Rotation")]
    public PlayerMove playerMove;


    [Header("Config")]
    public KeyCode pauseKey = KeyCode.Escape;
    public string mainMenuSceneName = "MainScene";
    [HideInInspector] public bool allowPause = true;

    [Header("Gameplay")]
    public MonoBehaviour[] scriptsToDisableOnPause;

    [Header("Audio")]
    public AudioSource[] audioSourcesToPause;

    public bool IsPaused { get; private set; }

    void Update()
    {
        if (!allowPause) return;

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
        if (pausePanel != null) pausePanel.SetActive(true);
        if (HUD != null) HUD.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var s in scriptsToDisableOnPause)
            if (s != null) s.enabled = false;

        foreach (var a in audioSourcesToPause)
            if (a != null) a.Pause();

        if(playerMove != null) 
            playerMove.canLook = false; // bloque rotation tête
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (HUD != null) HUD.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var s in scriptsToDisableOnPause)
            if (s != null) s.enabled = true;

        foreach (var a in audioSourcesToPause)
            if (a != null) a.UnPause();

        if(playerMove != null) 
            playerMove.canLook = true; // débloque rotation tête
    }


    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
