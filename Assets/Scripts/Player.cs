
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public bool canShoot;
    public bool isBuilding;
    public float mouseSensitivity = 2f;
    public GameObject prefab;

    private GameObject instance;
    private Camera playerCamera;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private int previewLayer;
    private int placedLayer = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isBuilding = false;
        playerCamera = GetComponent<Camera>();

        previewLayer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0) && canShoot && !isBuilding)
        {
            GameManager.Instance.Shoot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isBuilding)
            {
                Destroy(instance);
                instance = null;
                isBuilding = false;
            }
            else
            {
                canShoot = false;
                isBuilding = true;
                instance = Instantiate(prefab);

                SetLayerRecursively(instance, previewLayer);
            }
        }

        if (isBuilding && instance != null)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                instance.transform.position = hit.point;
                instance.transform.rotation =
                    Quaternion.LookRotation(hit.normal) *
                    Quaternion.Euler(90f, 0f, 0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && isBuilding && instance != null)
        {
            instance.transform.Rotate(0f, 45f, 0f);
        }

        if (Input.GetMouseButtonDown(0) && isBuilding && instance != null)
        {
            isBuilding = false;

            SetLayerRecursively(instance, placedLayer);

            PlayerPrefs.SetInt(
                GameManager.Instance.currentLevel + "_walls",
                PlayerPrefs.GetInt(GameManager.Instance.currentLevel + "_walls", 0) + 1
            );

            instance = null;
        }

        if (vertical != 0 || horizontal != 0 ||
            Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            float yAxis = 0f;

            if (Input.GetKey(KeyCode.Q)) yAxis -= 1f;
            if (Input.GetKey(KeyCode.E)) yAxis += 1f;

            transform.Translate(
                new Vector3(horizontal, yAxis, vertical) *
                Time.deltaTime * speed
            );

            canShoot = false;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position =
                GameManager.Instance.shootPosition.transform.position;

            canShoot = true;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (mouseX != 0 || mouseY != 0)
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
