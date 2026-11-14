using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.shooting = false;
        }
    }
}
