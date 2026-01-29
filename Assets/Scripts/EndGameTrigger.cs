using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameTrigger : MonoBehaviour
{
    [Header("Canvas de fin")]
    [SerializeField] private Canvas endCanvas;

    [Header("Options")]
    [SerializeField] private float delayBeforeMenu = 3f;

    private bool triggered = false;

    private void Awake()
    {
        if (endCanvas != null)
            endCanvas.gameObject.SetActive(false); // désactivé au départ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Player entré dans la zone !");
            
            if (endCanvas != null)
                endCanvas.gameObject.SetActive(true);

            // Affiche le curseur pour interagir avec les boutons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Pause le jeu
            Time.timeScale = 0f;

            StartCoroutine(ReturnToMenu());
        }
    }


    private IEnumerator ReturnToMenu()
    {
        float timer = 0f;
        while (timer < delayBeforeMenu)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("SceneMenu"); // changer selon le nom exact de ta scène menu
    }
}
