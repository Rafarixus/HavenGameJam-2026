using UnityEngine;

public class Ball : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.Finish();
        }
    }
}
