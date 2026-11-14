using TMPro;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private void Update()
    {
        if (GameManager.Instance)
        {
            timeText.text = GameManager.Instance.time.ToString();
        }
    }
}
