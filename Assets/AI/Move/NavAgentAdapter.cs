// /AI/Move/NavAgentAdapter.cs
using UnityEngine;
using UnityEngine.AI;
public class NavAgentAdapter : MonoBehaviour
{
    public NavMeshAgent agent;
    void Awake() { if (!agent) agent = GetComponent<NavMeshAgent>(); }
    public void Go(Vector3 p) { agent.isStopped = false; agent.SetDestination(p); }
    public bool Arrived => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    public void Stop() { agent.isStopped = true; }
}
