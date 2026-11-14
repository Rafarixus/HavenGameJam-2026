using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public bool canShoot;
    public float mouseSensitivity = 2f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical != 0 || horizontal != 0 || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            float yAxis = 0f;
            if (Input.GetKey(KeyCode.Q)) yAxis += -1f;
            if (Input.GetKey(KeyCode.E)) yAxis += 1f;

            transform.Translate(new Vector3(horizontal, yAxis, vertical) * Time.deltaTime * speed);
            canShoot = false;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position = GameManager.Instance.shootPosition.transform.position;
            canShoot = true;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}