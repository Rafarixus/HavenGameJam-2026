using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ball;
    public Transform playerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) {
            Instantiate(
                ball,
                playerCamera.transform.position + playerCamera.forward * 1f,
                playerCamera.transform.rotation
            );
        }
    }
}
