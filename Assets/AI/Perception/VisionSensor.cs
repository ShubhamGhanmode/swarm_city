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

  float lastSeenStamp = -999f;

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
}
