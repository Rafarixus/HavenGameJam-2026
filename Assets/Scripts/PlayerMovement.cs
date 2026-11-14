using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float forward;
    private float sideways;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        forward = Input.getAxis("Vertical");
        sideways = Input.getAxis("Horizontal");

        transform.position = new Vector3(forward, 0, sideways);
    }
}
