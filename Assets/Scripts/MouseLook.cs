using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity;
    private float mouseX;
    private float mouseY;

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(-mouseY * mouseSensitivity, mouseX * mouseSensitivity, 0);
    }
}
