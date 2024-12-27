using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float fastMoveMultiplier = 3f;
    public float rotationSpeed = 100f;
    public Light directionalLight; // Assign your directional light in the Inspector
    public float timeChangeSpeed = 1f;
    public float nightIntensity = 0.1f; // Intensity during the night
    public float dayIntensity = 1f; // Intensity during the day
    public Color dayColor = new Color(1f, 0.95f, 0.8f); // Warm light color for day
    public Color nightColor = new Color(0.5f, 0.5f, 0.6f); // Cool light color for night

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleTimeOfDay();
    }

    void HandleMovement()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= fastMoveMultiplier;
        }

        Vector3 direction = new Vector3(
            Input.GetAxis("Horizontal"), // A/D for left/right
            Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftControl) ? -1 : 0, // Space for up, Ctrl for down
            Input.GetAxis("Vertical") // W/S for forward/backward
        );

        transform.Translate(direction * speed * Time.deltaTime, Space.Self);
    }

    void HandleRotation()
    {
        float yaw = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float pitch = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, pitch, Space.Self);
    }

    void HandleTimeOfDay()
    {
        if (directionalLight != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                directionalLight.transform.Rotate(Vector3.right, scroll * timeChangeSpeed, Space.Self);
            }

            // Adjust intensity and color based on the sun's rotation
            float angle = directionalLight.transform.eulerAngles.x;
            if (angle > 180f) angle -= 360f; // Normalize angle to [-180, 180]

            float t = Mathf.InverseLerp(-90f, 90f, angle); // Map angle to [0, 1]
            directionalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, t);
            directionalLight.color = Color.Lerp(nightColor, dayColor, t);

            // Ensure no light from below the terrain
            if (angle < -90f || angle > 90f)
            {
                directionalLight.intensity = 0f;
            }
        }
    }
}
