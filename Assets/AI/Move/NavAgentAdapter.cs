
using UnityEngine;
using UnityEngine.AI;
public class NavAgentAdapter : MonoBehaviour
{
    public NavMeshAgent agent;
    void Awake() { if (!agent) agent = GetComponent<NavMeshAgent>(); }
    public void Go(Vector3 p) { agent.isStopped = false; agent.SetDestination(p); }
    public bool Arrived
    {
        get
        {
            if (agent.pathPending) return false;
            if (!agent.hasPath && agent.velocity.sqrMagnitude < 0.0025f) return true;
            float threshold = Mathf.Max(agent.stoppingDistance, 0.25f);
            return agent.remainingDistance <= threshold;
        }
    }
    public void Stop() { agent.isStopped = true; }
}
