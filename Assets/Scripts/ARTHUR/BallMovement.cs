using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 10f;
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
