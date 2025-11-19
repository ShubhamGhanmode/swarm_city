using UnityEngine;
using UnityEngine.AI;
public class AnimSpeedFromAgent : MonoBehaviour
{
    public Animator anim; public NavMeshAgent agent; public string param = "Speed";
    void Reset() { anim = GetComponent<Animator>(); agent = GetComponent<NavMeshAgent>(); }
    void Update() { if (anim && agent) anim.SetFloat(param, agent.velocity.magnitude); }
}