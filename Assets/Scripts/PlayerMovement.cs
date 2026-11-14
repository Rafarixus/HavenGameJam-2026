using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float forward;
    private float sideways;

    public float speed = 5f;

    void Update()
    {
        forward = Input.GetAxis("Vertical");
        sideways = Input.GetAxis("Horizontal");

        transform.Translate(new Vector3(sideways, 0, forward) * Time.deltaTime * speed);
    }
}