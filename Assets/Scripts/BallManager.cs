using UnityEngine;
using System.Collections.Generic;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }
    private List<GameObject> balls = new List<GameObject>();
    public GameObject ballPrefab;
    public GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShootBallFromPlayer(float strength)
    {
        GameObject ballParent = GameObject.Find("Balls");
        if (!ballPrefab)
        {
            ballParent = new GameObject();
            ballParent.name = "Balls";
        }

        GameObject newBall = Instantiate(ballPrefab, ballParent.transform);
        balls.Add(newBall);
        newBall.transform.position = player.transform.position;
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        Vector3 direction = playerCamera.transform.forward.normalized * strength;
        rb.AddForce(direction);
    }
}
