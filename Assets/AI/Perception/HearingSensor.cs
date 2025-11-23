// /AI/Perception/HearingSensor.cs
using UnityEngine;
public class HearingSensor : MonoBehaviour
{
    public Blackboard bb; public float maxAge = 3f;
    public float hearingRadiusMultiplier = 1f; // scale how far this actor can hear beyond the noise's own radius
    public float crouchSuppression = 0.35f; // reduces perceived intensity when player is crouched (if noise tagged "crouch")
    [Header("Debug")]
    public bool drawDebug = true;
    public Color gizmoColor = new Color(0.1f, 0.55f, 1f, 0.15f);
    float lastHeardStamp = -999f;

    void Awake() { if (!bb) bb = GetComponent<Blackboard>(); }
    void Reset() { bb = GetComponent<Blackboard>(); }
    void Update()
    {
        if (NoiseEventBus.TryGetStrongestNear(transform.position, maxAge, out var e, hearingRadiusMultiplier))
        {
            float k = (e.type == "crouch") ? crouchSuppression : 1f;
            if (k <= 0.01f) return;
            bb.lastHeard = e.pos;
            bb.suspicion = Mathf.Clamp01(bb.suspicion + 0.25f * k);
            lastHeardStamp = Time.time;
        }
        else if (bb.lastHeard.HasValue && Time.time - lastHeardStamp > maxAge)
        {
            // Clear outdated hints so the AI does not navigate toward long-gone sounds
            bb.lastHeard = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;
        Color prev = Gizmos.color;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, hearingRadiusMultiplier);
        if (bb && bb.lastHeard.HasValue)
        {
            Gizmos.DrawSphere(bb.lastHeard.Value, 0.2f);
            Gizmos.DrawLine(transform.position, bb.lastHeard.Value);
        }
        Gizmos.color = prev;
    }
}
