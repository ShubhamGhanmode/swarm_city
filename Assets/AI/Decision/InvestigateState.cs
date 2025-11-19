// /AI/Decision/InvestigateState.cs
using UnityEngine;
public class InvestigateState : IState
{
    readonly NavAgentAdapter nav; readonly Blackboard bb; readonly System.Action done;
    public InvestigateState(NavAgentAdapter nav, Blackboard bb, System.Action done) { this.nav = nav; this.bb = bb; this.done = done; }
    public void Enter() { if (bb.lastHeard.HasValue) nav.Go(bb.lastHeard.Value); }
    public void Tick(float dt)
    {
        if (nav.Arrived) { bb.lastHeard = null; done?.Invoke(); }
    }
    public void Exit() { }
}
