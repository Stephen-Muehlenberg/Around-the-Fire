using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  [SerializeField] private GameObject menuPanel;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (menuPanel.activeInHierarchy)
        Resume();
      else
        Show();
    }
  }

  public void Show()
  {
    Time.timeScale = 0;
    menuPanel.SetActive(true);
  }

  public void Resume()
  {
    menuPanel.SetActive(false);
    Time.timeScale = 1;
  }

  public void Save()
  {

  }

  public void Load()
  {

  }

  public void Settings()
  {

  }

  public void MainMenu()
  {
    SceneManager.LoadScene("Main Menu");
  }

  public void Exit()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
  }
}
