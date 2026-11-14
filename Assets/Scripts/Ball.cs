using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Ball : MonoBehaviour
{
    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        // Sweep the ball between physics steps so fast shots cannot skip thin walls.
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.Finish();
        }
    }
}
