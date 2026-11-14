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
        GameObject newBall = Instantiate(ballPrefab, shootPosition.transform.position + player.transform.forward.normalized, player.transform.rotation);
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
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
    }
}
