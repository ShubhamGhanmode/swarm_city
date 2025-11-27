
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VisionSensor : MonoBehaviour {
  public Blackboard bb;
  public Transform target;
  public float range = 12f;
  public float fov = 100f;
  public LayerMask losMask;        
  public float memory = 0.5f;      
  public bool requirePositiveLOS = true; 
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
          
          if (Physics.Raycast(eye, (target.position - eye).normalized, out var hit, range, losMask)){
            if (hit.transform == target){
              
              bb.lastSeen = target.position;
              lastSeenStamp = Time.time;
              bb.suspicion = 1f;
              seenThisFrame = true;
            }
          } else if (!requirePositiveLOS) {
            
            bb.lastSeen = target.position;
            lastSeenStamp = Time.time;
            bb.suspicion = 1f;
            seenThisFrame = true;
          }
        }
      }
    }

    
    if (!seenThisFrame && Time.time - lastSeenStamp > memory){
      bb.lastSeen = null;
    }
  }

  void OnDrawGizmosSelected(){
    if (!drawDebug) return;
    Vector3 eye = transform.position + Vector3.up * 1.6f;
    Vector3 flatForward = transform.forward; flatForward.y = 0f;
    if (flatForward.sqrMagnitude < 0.001f) flatForward = transform.forward;
    flatForward.Normalize();

    Color prev = Gizmos.color;
#if UNITY_EDITOR
    Color prevHandlesColor = Handles.color;
    var prevZTest = Handles.zTest;
    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

    
    Color fovFill = gizmoOccludedColor; fovFill.a *= 0.35f;
    Vector3 startDir = Quaternion.Euler(0f, -fov * 0.5f, 0f) * flatForward;
    Handles.color = fovFill;
    Handles.DrawSolidArc(eye, Vector3.up, startDir, fov, range);

    
    Handles.color = new Color(gizmoOccludedColor.r, gizmoOccludedColor.g, gizmoOccludedColor.b, gizmoOccludedColor.a * 1.35f);
    Handles.DrawWireDisc(eye, Vector3.up, range);
#else
    Gizmos.color = gizmoOccludedColor;
    Gizmos.DrawWireSphere(eye, range);
#endif

    Vector3 left = Quaternion.Euler(0f, -fov * 0.5f, 0f) * flatForward;
    Vector3 right = Quaternion.Euler(0f, fov * 0.5f, 0f) * flatForward;
    Gizmos.color = new Color(gizmoOccludedColor.r, gizmoOccludedColor.g, gizmoOccludedColor.b, Mathf.Clamp01(gizmoOccludedColor.a + 0.15f));
    Gizmos.DrawRay(eye, left * range);
    Gizmos.DrawRay(eye, right * range);
    Gizmos.DrawRay(eye, flatForward * range * 0.9f);

    
    if (bb && bb.lastSeen.HasValue){
#if UNITY_EDITOR
      Handles.color = gizmoVisibleColor;
      Handles.DrawDottedLine(eye, bb.lastSeen.Value, 4f);
      Handles.SphereHandleCap(0, bb.lastSeen.Value, Quaternion.identity, 0.3f, EventType.Repaint);
      Handles.color = new Color(gizmoVisibleColor.r, gizmoVisibleColor.g, gizmoVisibleColor.b, gizmoVisibleColor.a * 0.55f);
      Handles.DrawSolidDisc(new Vector3(bb.lastSeen.Value.x, eye.y, bb.lastSeen.Value.z), Vector3.up, 0.22f);
#else
      Gizmos.color = gizmoVisibleColor;
      Gizmos.DrawSphere(bb.lastSeen.Value, 0.25f);
      Gizmos.DrawLine(eye, bb.lastSeen.Value);
#endif
    }
    Gizmos.color = prev;
#if UNITY_EDITOR
    Handles.color = prevHandlesColor;
    Handles.zTest = prevZTest;
#endif
  }
}
