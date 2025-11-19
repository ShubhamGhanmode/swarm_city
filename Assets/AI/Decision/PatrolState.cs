// /AI/Decision/PatrolState.cs
using UnityEngine;
public class PatrolState : IState
{
    readonly NavAgentAdapter nav; readonly Vector3[] wps; int i; readonly System.Func<bool> onStimulus;
    public PatrolState(NavAgentAdapter nav, Vector3[] wps, System.Func<bool> onStimulus) { this.nav = nav; this.wps = wps; this.onStimulus = onStimulus; }
    public void Enter() { if (wps.Length > 0) nav.Go(wps[i]); }
    public void Tick(float dt)
    {
        if (onStimulus()) return;
        if (nav.Arrived && wps.Length > 0) { i = (i + 1) % wps.Length; nav.Go(wps[i]); }
    }
    public void Exit() { }
}
