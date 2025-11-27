
using UnityEngine;
public class InvestigateState : IState
{
    readonly NavAgentAdapter nav; readonly Blackboard bb; readonly System.Action done; readonly System.Action onSight;
    Vector3? currentTarget;
    const float retargetThreshold = 0.35f;

    public InvestigateState(NavAgentAdapter nav, Blackboard bb, System.Action done, System.Action onSight = null) { this.nav = nav; this.bb = bb; this.done = done; this.onSight = onSight; }
    public void Enter()
    {
        currentTarget = bb.lastHeard;
        if (currentTarget.HasValue) nav.Go(currentTarget.Value);
    }
    public void Tick(float dt)
    {
        
        if (bb.lastSeen.HasValue)
        {
            onSight?.Invoke();
            return;
        }
        
        if (bb.lastHeard.HasValue)
        {
            var latest = bb.lastHeard.Value;
            if (!currentTarget.HasValue || Vector3.Distance(currentTarget.Value, latest) > retargetThreshold)
            {
                currentTarget = latest;
                nav.Go(latest);
            }
        }

        
        if (nav.Arrived && (!bb.lastHeard.HasValue ||
                            (currentTarget.HasValue && Vector3.Distance(currentTarget.Value, bb.lastHeard.Value) <= retargetThreshold)))
        {
            bb.lastHeard = null;
            done?.Invoke();
        }
    }
    public void Exit() { currentTarget = null; }
}
