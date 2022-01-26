using UnityEngine;

public class MainMenu : MonoBehaviour
{
  public void NewGame()
  {
    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Travel");
  }
}
