using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class MoveObject : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 400f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        LookAround();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Optional: Jump and crouch (Space and LeftShift)
        if (Input.GetKey(KeyCode.Space))
        {
            move += Vector3.up;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            move += Vector3.down;
        }

        transform.position += move * moveSpeed * Time.deltaTime;
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical rotation

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
