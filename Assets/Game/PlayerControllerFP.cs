// Assets/Game/PlayerControllerFP.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerFP : MonoBehaviour
{
    [Header("References")]
    public Transform cam;          // POVCamera transform
    public Transform povCamera;    // same as cam (kept separate if you prefer)

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float crouchSpeed = 2.5f;
    public float gravity = -20f;

    [Header("Crouch")]
    public float standHeight = 2.0f;
    public float crouchHeight = 1.2f;
    public float standCameraY = 1.6f;
    public float crouchCameraY = 1.0f;
    public bool crouchSilent = true;   // no noise while crouched

    [Header("Noise")]
    public float footstepInterval = 0.45f; // seconds between steps at walkSpeed
    public float walkNoiseIntensity = 12f, walkNoiseRadius = 20f;

    CharacterController controller;
    Vector3 velocity;
    float stepTimer;
    bool isCrouched;
    float initialCameraY;

    void Awake() { controller = GetComponent<CharacterController>(); }

    void Update()
    {
        // 1) Camera-relative WASD
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.ClampMagnitude(new Vector3(h, 0, v), 1f);

        Vector3 fwd = cam.forward; fwd.y = 0; fwd.Normalize();
        Vector3 right = cam.right; right.y = 0; right.Normalize();
        Vector3 moveDir = fwd * input.z + right * input.x;

        float speed = isCrouched ? crouchSpeed : walkSpeed;
        controller.Move(moveDir * speed * Time.deltaTime);

        // 2) Crouch (hold LeftCtrl or C)
        bool crouchHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        if (crouchHeld != isCrouched) { isCrouched = crouchHeld; SetCrouch(isCrouched); }

        // 3) Jump + landing stick
        if (controller.isGrounded)
        {
            if (velocity.y < 0) velocity.y = -2f; // keep grounded

            // 4) Footstep noise (disabled when crouched if crouchSilent)
            bool moving = moveDir.sqrMagnitude > 0.01f && speed > 0.1f;
            if (moving)
            {
                stepTimer += Time.deltaTime;
                float interval = footstepInterval * (walkSpeed / speed); // slower speed -> slower steps
                if (stepTimer >= interval)
                {
                    stepTimer = 0f;
                    if (!(crouchSilent && isCrouched))
                    {
                        string type = isCrouched ? "crouch" : "footstep";
                        float intensity = isCrouched ? walkNoiseIntensity * 0.5f : walkNoiseIntensity;
                        float radius = isCrouched ? walkNoiseRadius * 0.6f : walkNoiseRadius;
                        NoiseEventBus.Raise(transform.position, intensity, radius, type);
                    }
                }
            }
            else stepTimer = 0f;
        }

        // 5) Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void SetCrouch(bool crouch)
    {
        controller.height = crouch ? crouchHeight : standHeight;
        controller.center = new Vector3(0, controller.height * 0.5f, 0);
        // Move both povCamera and cam in case only one is assigned
        float targetY = crouch ? crouchCameraY : standCameraY;
        if (Mathf.Approximately(standCameraY, 0f) && Mathf.Approximately(initialCameraY, 0f))
            initialCameraY = targetY;
        ApplyCameraHeight(povCamera, targetY);
        if (cam != povCamera) ApplyCameraHeight(cam, targetY);
    }

    void ApplyCameraHeight(Transform t, float y)
    {
        if (!t) return;
        var lp = t.localPosition;
        lp.y = y;
        t.localPosition = lp;
    }
}
