// /AI/Decision/InvestigateState.cs
using UnityEngine;
public class InvestigateState : IState
{
    readonly NavAgentAdapter nav; readonly Blackboard bb; readonly System.Action done;
    Vector3? currentTarget;
    const float retargetThreshold = 0.35f;

    public InvestigateState(NavAgentAdapter nav, Blackboard bb, System.Action done) { this.nav = nav; this.bb = bb; this.done = done; }
    public void Enter()
    {
        currentTarget = bb.lastHeard;
        if (currentTarget.HasValue) nav.Go(currentTarget.Value);
    }
    public void Tick(float dt)
    {
        // Keep chasing the freshest sound location while the player is still making noise.
        if (bb.lastHeard.HasValue)
        {
            var latest = bb.lastHeard.Value;
            if (!currentTarget.HasValue || Vector3.Distance(currentTarget.Value, latest) > retargetThreshold)
            {
                currentTarget = latest;
                nav.Go(latest);
            }
        }

        // Exit only after reaching the most recent noise and no new sounds are coming in.
        if (nav.Arrived && (!bb.lastHeard.HasValue ||
                            (currentTarget.HasValue && Vector3.Distance(currentTarget.Value, bb.lastHeard.Value) <= retargetThreshold)))
        {
            bb.lastHeard = null;
            done?.Invoke();
        }
    }
    public void Exit() { currentTarget = null; }
}
