using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject ballPrefab;
    public GameObject player;
    public float strength;
    public int score;
    public float time;
    public bool shooting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Physics.IgnoreLayerCollision(3, 6);
    }

    private void Update()
    {
        if (shooting) time = time + Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject newBall = Instantiate(ballPrefab);
        newBall.transform.position = player.transform.position;
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        Vector3 direction = playerCamera.transform.forward.normalized * strength;
        rb.AddForce(direction);
        time = 0;
        shooting = true;
    }
}
