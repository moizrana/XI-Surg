using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        // Move
        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float y = 0;

        if (Input.GetKey(KeyCode.E)) y += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) y -= moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(x, y, z));

        // Look
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f);
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
