using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject ballPrefab;
    public GameObject player;
    public float strength = 250f;
    public int score;
    public float time;
    public bool shooting;
    private Camera playerCamera;

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

    private void Start()
    {
        playerCamera = player.GetComponentInChildren<Camera>();
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
        GameObject newBall = Instantiate(ballPrefab, playerCamera.transform.position + playerCamera.transform.forward.normalized, playerCamera.transform.rotation);
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        Vector3 direction = playerCamera.transform.forward.normalized * strength;
        rb.AddForce(direction);
        time = 0;
        shooting = true;
    }
}
