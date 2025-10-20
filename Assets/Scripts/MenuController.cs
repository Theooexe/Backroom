using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
   public void ChangeScene(string _sceneName)
   {
      SceneManager.LoadScene(_sceneName);
   }

   public void Quit()
   {
      Application.Quit();
   }
}
