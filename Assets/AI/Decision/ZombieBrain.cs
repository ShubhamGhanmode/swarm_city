// /AI/Decision/ZombieBrain.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class ZombieBrain : MonoBehaviour
{
    public Blackboard bb; public NavAgentAdapter nav; public Transform[] waypoints; public VisionSensor vision; public Transform player;

    [Header("Auto Patrol")]
    public int autoWaypointCount = 4;
    public float autoWaypointRadius = 8f;
    public float autoWaypointSampleRadius = 2f;

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
        var wps = BuildPatrolPoints();

        fsm = new StateMachine();
        patrol = new PatrolState(nav, wps, () =>
        {
            if (bb.lastHeard.HasValue) { SetState(investigate, InvestigateName); return true; }
            if (bb.suspicion >= 0.8f && bb.lastSeen.HasValue) { SetState(chase, ChaseName); return true; }
            return false;
        });
        investigate = new InvestigateState(nav, bb,
            done: () => SetState(patrol, PatrolName),
            onSight: () => { bb.suspicion = 1f; SetState(chase, ChaseName); });
        search = new SearchState(nav, bb, radius: 6f, probes: 6, dwell: 1f);
        search.onDone = () => { bb.suspicion = 0f; bb.lastSeen = null; SetState(patrol, PatrolName); };
        chase = new ChaseState(nav, bb, player, (lastPos) =>
        {
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

    Vector3[] BuildPatrolPoints()
    {
        var manual = waypoints?.Where(w => w != null).Select(w => w.position).ToArray();
        if (manual != null && manual.Length > 0) return manual;
        return GenerateRandomPatrolPoints();
    }

    Vector3[] GenerateRandomPatrolPoints()
    {
        int count = Mathf.Max(1, autoWaypointCount);
        float radius = Mathf.Max(0.5f, autoWaypointRadius);
        float sampleRadius = Mathf.Clamp(autoWaypointSampleRadius, 0.25f, radius);

        var points = new List<Vector3>();
        Vector3 center = transform.position;
        int attempts = count * 4; // a few tries per desired point to find the navmesh

        for (int i = 0; i < attempts && points.Count < count; i++)
        {
            var offset2D = Random.insideUnitCircle * radius;
            var candidate = center + new Vector3(offset2D.x, 0f, offset2D.y);
            if (NavMesh.SamplePosition(candidate, out var hit, sampleRadius, NavMesh.AllAreas))
                points.Add(hit.position);
        }

        if (points.Count == 0) points.Add(center);
        return points.ToArray();
    }
}
