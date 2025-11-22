// /AI/Decision/ZombieBrain.cs
using System.Linq;
using UnityEngine;
public class ZombieBrain : MonoBehaviour
{
    public Blackboard bb; public NavAgentAdapter nav; public Transform[] waypoints; public VisionSensor vision; public Transform player;
    StateMachine fsm; PatrolState patrol; InvestigateState investigate; ChaseState chase; SearchState search;

    public string CurrentStateName { get; private set; } = "None";
    const string PatrolName = "Patrol";
    const string InvestigateName = "Investigate";
    const string ChaseName = "Chase";
    const string SearchName = "Search";

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
            if (bb.lastHeard.HasValue) { SetState(investigate, InvestigateName); return true; }
            if (bb.suspicion >= 0.8f && bb.lastSeen.HasValue) { SetState(chase, ChaseName); return true; }
            return false;
        });
        investigate = new InvestigateState(nav, bb,
            done: () => SetState(patrol, PatrolName),
            onSight: () => { bb.suspicion = 1f; SetState(chase, ChaseName); });
        search = new SearchState(nav, bb, radius: 6f, probes: 6, dwell: 1f);
        search.onDone = () => { bb.suspicion = 0f; bb.lastSeen = null; SetState(patrol, PatrolName); };
        chase = new ChaseState(nav, bb, player, (lastPos) => {
            bb.suspicion = Mathf.Clamp01(bb.suspicion - 0.2f);
            if (lastPos.HasValue) { bb.lastSeen = lastPos; SetState(search, SearchName); }
            else { bb.lastSeen = null; SetState(patrol, PatrolName); }
        });
    }
    void Start() { SetState(patrol, PatrolName); }
    void Update()
    {
        // suspicion decay when calm
        if (!bb.lastSeen.HasValue && !bb.lastHeard.HasValue)
            bb.suspicion = Mathf.Max(0f, bb.suspicion - 0.2f * Time.deltaTime);
        fsm.Tick(Time.deltaTime);
    }

    void SetState(IState next, string name)
    {
        CurrentStateName = name;
        fsm.Set(next);
    }
}
