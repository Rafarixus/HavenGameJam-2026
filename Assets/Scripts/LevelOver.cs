using TMPro;
using UnityEngine;

public class LevelOver : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI triesText;
    public TextMeshProUGUI wallsText;


    public void whatever()
    {
        timeText.text = GameManager.Instance.GetTime().ToString("F2");
        highscoreText.text = PlayerPrefs.GetInt(GameManager.Instance.currentLevel + "_highscore", 0).ToString();
        triesText.text = PlayerPrefs.GetInt(GameManager.Instance.currentLevel + "_tries", 0).ToString();
        wallsText.text = PlayerPrefs.GetInt(GameManager.Instance.currentLevel + "_walls", 0).ToString();
    }
}
