// Assets/AI/Decision/SearchState.cs
using UnityEngine;

public class SearchState : IState
{
    readonly NavAgentAdapter nav; readonly Blackboard bb;
    readonly float radius; readonly int probes; readonly float dwell;
    Vector3 center; int i; float dwellTimer;

    public System.Action onDone;

    public SearchState(NavAgentAdapter nav, Blackboard bb, float radius = 6f, int probes = 6, float dwell = 1f)
    {
        this.nav = nav; this.bb = bb; this.radius = radius; this.probes = Mathf.Max(3, probes); this.dwell = dwell;
    }

    public void Enter()
    {
        center = bb.lastSeen ?? bb.lastHeard ?? nav.agent.transform.position;
        i = 0; dwellTimer = 0f;
        nav.Go(ProbePos(i));
    }

    public void Tick(float dt)
    {
        // if new sound arrived, let brain switch out
        if (bb.lastHeard.HasValue && Vector3.Distance(center, bb.lastHeard.Value) > 0.5f)
        {
            center = bb.lastHeard.Value; i = 0; nav.Go(ProbePos(i));
            return;
        }
        // If vision reacquired, stop searching (brain will change state)
        if (bb.lastSeen.HasValue) { onDone?.Invoke(); return; }
        if (nav.Arrived)
        {
            dwellTimer += dt;
            if (dwellTimer >= dwell)
            {
                dwellTimer = 0f;
                i++;
                if (i >= probes) { onDone?.Invoke(); return; }
                nav.Go(ProbePos(i));
            }
        }
    }

    public void Exit() { }

    Vector3 ProbePos(int k)
    {
        float ang = (k / (float)probes) * Mathf.PI * 2f;
        Vector3 dir = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));
        return center + dir * radius;
    }
}
