using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOver : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI triesText;
    public TextMeshProUGUI wallsText;


    public void whatever()
    {
        timeText.text = GameManager.Instance.GetTime().ToString("F2");
        highscoreText.text = PlayerPrefs.GetFloat(GameManager.Instance.currentLevel + "_highscore", 0).ToString();
        triesText.text = PlayerPrefs.GetInt(GameManager.Instance.currentLevel + "_tries", 0).ToString();
        wallsText.text = PlayerPrefs.GetInt(GameManager.Instance.currentLevel + "_walls", 0).ToString();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("currentLevel", 1) + 1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(GameManager.Instance.currentLevel);
    }
    public void LoadLevel(int id)
    {
        SceneManager.LoadScene(id);
    }

}
