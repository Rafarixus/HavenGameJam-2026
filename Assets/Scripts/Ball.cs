using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.shooting = false;
        }
    }
}
