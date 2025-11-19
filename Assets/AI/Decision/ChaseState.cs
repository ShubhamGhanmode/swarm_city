// Assets/AI/Decision/ChaseState.cs
using UnityEngine;

public class ChaseState : IState
{
    private readonly NavAgentAdapter nav;
    private readonly Blackboard bb;
    private readonly Transform target;
    private readonly System.Action<Vector3?> onLost;

    private float loseTimer = 0f;
    private const float loseDelay = 2f; // seconds without sight before giving up
    private Vector3? lastKnownSight;

    public ChaseState(NavAgentAdapter nav, Blackboard bb, Transform target, System.Action<Vector3?> onLost)
    {
        this.nav = nav;
        this.bb = bb;
        this.target = target;
        this.onLost = onLost;
    }

    public void Enter()
    {
        loseTimer = 0f;
        lastKnownSight = null;
    }

    public void Tick(float dt)
    {
        if (target != null)
            nav.Go(target.position);

        if (bb.lastSeen.HasValue)
        {
            loseTimer = 0f;
            lastKnownSight = bb.lastSeen;
        }
        else
        {
            loseTimer += dt;
            if (loseTimer >= loseDelay)
                onLost?.Invoke(lastKnownSight);
        }
    }

    public void Exit() { }
}
