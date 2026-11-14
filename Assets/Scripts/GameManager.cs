using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Generall Settings")]
    public static GameManager Instance { get; private set; }
    public GameObject ballPrefab;
    public float strength = 1000f;
    public GameObject player;

    [Header("Level Settings")]
    public GameObject shootPosition;

    private float time;
    private bool shooting;
    private Camera playerCamera;
    private GameObject currentBall;
    public int currentLevel;

    public GameObject finishUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Physics.IgnoreLayerCollision(3, 6);

        finishUI.SetActive(false);
    }

    private void Update()
    {
        if (shooting) time = time + Time.deltaTime;
    }

    public void Shoot()
    {
        PlayerPrefs.SetInt(currentLevel + "_tries", PlayerPrefs.GetInt(currentLevel + "_tries", 0) + 1);
        if (currentBall) Destroy(currentBall);
        currentBall = Instantiate(ballPrefab, shootPosition.transform.position + player.transform.forward.normalized, player.transform.rotation);
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        Vector3 direction = player.transform.forward.normalized * strength;
        rb.AddForce(direction);
        time = 0;
        shooting = true;
    }

    public float GetTime()
    {
        return time;
    }

    public void Finish()
    {
        shooting = false;
        finishUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        PlayerPrefs.SetInt(currentLevel + "_highscore", 0);
        finishUI.GetComponent<LevelOver>().whatever();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<Player>().enabled = false;
    }
}
