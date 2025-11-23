// Assets/AI/Perception/VisionSensor.cs
using UnityEngine;

public class VisionSensor : MonoBehaviour {
  public Blackboard bb;
  public Transform target;
  public float range = 12f;
  public float fov = 100f;
  public LayerMask losMask;        // MUST include the target's layer and the Obstacles layer
  public float memory = 0.5f;      // seconds to remember last seen
  public bool requirePositiveLOS = true; // if true, requires a clean hit to the target; if false, falls back to overlap check (for debugging)
  [Header("Debug")]
  public bool drawDebug = true;
  public Color gizmoVisibleColor = new Color(0f, 1f, 0.4f, 0.2f);
  public Color gizmoOccludedColor = new Color(1f, 0.6f, 0f, 0.15f);

  float lastSeenStamp = -999f;

  void Awake(){ if (!bb) bb = GetComponent<Blackboard>(); }

  void Reset(){ bb = GetComponent<Blackboard>(); }

  void Update(){
    bool seenThisFrame = false;
    if (target){
      Vector3 eye = transform.position + Vector3.up * 1.6f;
      Vector3 to = target.position - eye; to.y = 0f;

      if (to.magnitude <= range){
        float ang = Vector3.Angle(transform.forward, to);
        if (ang <= fov * 0.5f){
          // Raycast that hits either target or an occluder
          if (Physics.Raycast(eye, (target.position - eye).normalized, out var hit, range, losMask)){
            if (hit.transform == target){
              // visible
              bb.lastSeen = target.position;
              lastSeenStamp = Time.time;
              bb.suspicion = Mathf.Clamp01(bb.suspicion + 0.6f * Time.deltaTime);
              seenThisFrame = true;
            }
          } else if (!requirePositiveLOS) {
            // fallback: within cone but LOS mask failed (e.g. mask misconfigured); still mark as seen to aid debugging
            bb.lastSeen = target.position;
            lastSeenStamp = Time.time;
            bb.suspicion = Mathf.Clamp01(bb.suspicion + 0.6f * Time.deltaTime);
            seenThisFrame = true;
          }
        }
      }
    }

    // Clear lastSeen when not visible for 'memory' seconds
    if (!seenThisFrame && Time.time - lastSeenStamp > memory){
      bb.lastSeen = null;
    }
  }

  void OnDrawGizmosSelected(){
    if (!drawDebug) return;
    Vector3 eye = transform.position + Vector3.up * 1.6f;

    // Draw FOV cone and reach
    Color prev = Gizmos.color;
    Gizmos.color = gizmoOccludedColor;
    Gizmos.DrawWireSphere(eye, range);
    Vector3 left = Quaternion.Euler(0f, -fov * 0.5f, 0f) * transform.forward;
    Vector3 right = Quaternion.Euler(0f, fov * 0.5f, 0f) * transform.forward;
    Gizmos.DrawRay(eye, left * range);
    Gizmos.DrawRay(eye, right * range);

    // Draw last seen marker if any
    if (bb && bb.lastSeen.HasValue){
      Gizmos.color = gizmoVisibleColor;
      Gizmos.DrawSphere(bb.lastSeen.Value, 0.25f);
      Gizmos.DrawLine(eye, bb.lastSeen.Value);
    }
    Gizmos.color = prev;
  }
}
