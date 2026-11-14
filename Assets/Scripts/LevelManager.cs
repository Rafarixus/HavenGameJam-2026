using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void StartGame()
    {
        if (PlayerPrefs.GetInt("currentLevel", 1) == 0) PlayerPrefs.SetInt("currentLevel", 1);
        SceneManager.LoadScene(PlayerPrefs.GetInt("currentLevel", 1));
    }

    public void LoadLevel(int id)
    {
        SceneManager.LoadScene(id);
    }
    public void MainMenu() {
        SceneManager.LoadScene(0);
    }
    public void Quit() {
        Application.Quit();
    }
    public void ReloadScene()
    {
        LoadLevel(GameManager.Instance.currentLevel);
    }
}
