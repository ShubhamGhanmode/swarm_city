// /AI/Perception/HearingSensor.cs
using UnityEngine;
public class HearingSensor : MonoBehaviour
{
    public Blackboard bb; public float maxAge = 3f;
    public float hearingRadiusMultiplier = 1f; // scale how far this actor can hear beyond the noise's own radius
    public float crouchSuppression = 0.35f; // reduces perceived intensity when player is crouched (if noise tagged "crouch")
    void Reset() { bb = GetComponent<Blackboard>(); }
    void Update()
    {
        if (NoiseEventBus.TryGetStrongestNear(transform.position, maxAge, out var e, hearingRadiusMultiplier))
        {
            float k = (e.type == "crouch") ? crouchSuppression : 1f;
            if (k <= 0.01f) return;
            bb.lastHeard = e.pos;
            bb.suspicion = Mathf.Clamp01(bb.suspicion + 0.25f * k);
        }
    }
}
