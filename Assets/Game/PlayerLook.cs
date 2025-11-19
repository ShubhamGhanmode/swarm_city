using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 200f;
    public Transform playerBody;  // assign your Player capsule
    float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // vertical look (camera pitch)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // horizontal look (player yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
