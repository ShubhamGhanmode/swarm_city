// /AI/Decision/ZombieBrain.cs
using System.Linq;
using UnityEngine;
public class ZombieBrain : MonoBehaviour
{
    public Blackboard bb; public NavAgentAdapter nav; public Transform[] waypoints; public VisionSensor vision; public Transform player;
    StateMachine fsm; PatrolState patrol; InvestigateState investigate; ChaseState chase; SearchState search;

    void Awake()
    {
        if (!bb) bb = GetComponent<Blackboard>();
        if (!nav) nav = GetComponent<NavAgentAdapter>();
        if (!vision) vision = GetComponent<VisionSensor>();
        if (!player) { var p = GameObject.FindWithTag("Player"); if (p) player = p.transform; }
        if (vision)
        {
            if (!vision.bb) vision.bb = bb;
            vision.target = player;
        }
        var wps = waypoints?.Select(w => w.position).ToArray() ?? new Vector3[0];

        fsm = new StateMachine();
        patrol = new PatrolState(nav, wps, () => {
            if (bb.lastHeard.HasValue) { fsm.Set(investigate); return true; }
            if (bb.suspicion >= 0.8f && bb.lastSeen.HasValue) { fsm.Set(chase); return true; }
            return false;
        });
        investigate = new InvestigateState(nav, bb, () => fsm.Set(patrol));
        search = new SearchState(nav, bb, radius: 6f, probes: 6, dwell: 1f);
        search.onDone = () => { bb.suspicion = 0f; bb.lastSeen = null; fsm.Set(patrol); };
        chase = new ChaseState(nav, bb, player, (lastPos) => {
            bb.suspicion = Mathf.Clamp01(bb.suspicion - 0.2f);
            if (lastPos.HasValue) { bb.lastSeen = lastPos; fsm.Set(search); }
            else { bb.lastSeen = null; fsm.Set(patrol); }
        });
    }
    void Start() { fsm.Set(patrol); }
    void Update()
    {
        // suspicion decay when calm
        if (!bb.lastSeen.HasValue && !bb.lastHeard.HasValue)
            bb.suspicion = Mathf.Max(0f, bb.suspicion - 0.2f * Time.deltaTime);
        fsm.Tick(Time.deltaTime);
    }

}
