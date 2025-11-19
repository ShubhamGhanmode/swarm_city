using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    public float stepInterval = 0.6f;
    public float noiseIntensity = 12f;
    public float noiseRadius = 20f;
    public bool crouchSilent = true;
    public KeyCode crouchKey = KeyCode.LeftControl;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        // Input axes can have tiny drift; use a deadzone so we don't emit noise while idle.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool moving = Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f;
        bool crouched = Input.GetKey(crouchKey);
        if (moving && timer >= stepInterval)
        {
            if (!(crouchSilent && crouched))
                NoiseEventBus.Raise(transform.position, noiseIntensity, noiseRadius, "footstep");
            timer = 0f;
        }
    }
}
