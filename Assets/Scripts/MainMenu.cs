using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public void NewGame()
  {
    SceneManager.LoadSceneAsync("Town");
  }
}
