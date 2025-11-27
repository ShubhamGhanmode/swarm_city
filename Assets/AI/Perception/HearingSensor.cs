
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class HearingSensor : MonoBehaviour
{
    public Blackboard bb; public float maxAge = 3f;
    public float hearingRadiusMultiplier = 1f; 
    public float crouchSuppression = 0.35f; 
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
            
            bb.lastHeard = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;
        Vector3 pos = transform.position;
        Color prev = Gizmos.color;
#if UNITY_EDITOR
        Color prevHandlesColor = Handles.color;
        var prevZTest = Handles.zTest;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        Color fill = gizmoColor; fill.a *= 0.28f;
        Handles.color = fill;
        Handles.DrawSolidDisc(pos, Vector3.up, hearingRadiusMultiplier);

        Handles.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoColor.a * 1.2f);
        Handles.DrawWireDisc(pos, Vector3.up, hearingRadiusMultiplier);

        float crouchRing = hearingRadiusMultiplier * Mathf.Clamp01(crouchSuppression);
        if (crouchRing > 0.05f)
        {
            Color crouchColor = new Color(0.95f, 0.45f, 0.2f, gizmoColor.a * 0.85f);
            Handles.color = crouchColor;
            Handles.DrawWireDisc(pos, Vector3.up, crouchRing);
        }
#else
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(pos, hearingRadiusMultiplier);
#endif
        if (bb && bb.lastHeard.HasValue)
        {
            Vector3 heard = bb.lastHeard.Value;
#if UNITY_EDITOR
            Handles.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, Mathf.Clamp01(gizmoColor.a + 0.2f));
            Handles.DrawDottedLine(pos, heard, 4f);
            Handles.SphereHandleCap(0, heard, Quaternion.identity, 0.24f, EventType.Repaint);
#else
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(heard, 0.2f);
            Gizmos.DrawLine(pos, heard);
#endif
        }
        Gizmos.color = prev;
#if UNITY_EDITOR
        Handles.color = prevHandlesColor;
        Handles.zTest = prevZTest;
#endif
    }
}
