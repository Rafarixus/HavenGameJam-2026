using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public bool canShoot;
    public bool isBuilding;
    public float mouseSensitivity = 2f;
    public GameObject prefab;
    private GameObject instance;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isBuilding = false;
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0) && canShoot && !isBuilding)
        {
            GameManager.Instance.Shoot();
        }

        if (Input.GetMouseButtonDown(1)) {
            if (isBuilding) isBuilding = false;
            if (!isBuilding)
            {
                canShoot = false;
                isBuilding = true;
                instance = Instantiate(prefab);
            }
        }

        if (isBuilding)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                instance.transform.position = hit.point;
                instance.transform.rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && isBuilding)
        {
            instance.transform.Rotate(0f, 45f, 0f);
        }

        if (Input.GetMouseButtonDown(0) && isBuilding)
        {
            isBuilding = false;
            instance = null;
        }

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