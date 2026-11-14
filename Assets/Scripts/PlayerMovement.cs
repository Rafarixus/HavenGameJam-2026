using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float forward;
    private float sideways;

    public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        forward = Input.GetAxis("Vertical");
        sideways = Input.GetAxis("Horizontal");

        transform.position += new Vector3(forward, 0, sideways) * Time.deltaTime * speed;
    }
}
