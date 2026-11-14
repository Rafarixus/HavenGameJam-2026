using UnityEngine;

public class BallStart : MonoBehaviour
{
    public GameObject ball;
    public float force = 5f;
    public Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        Vector3 force = new Vector3(0, 0, 5);
    //        ball.AddForce(force, ForceMode.Impulse);
    //    }
    //}
}
