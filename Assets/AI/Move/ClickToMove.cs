using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    public LayerMask groundMask;
    NavMeshAgent agent; Camera cam;
    void Awake() { agent = GetComponent<NavMeshAgent>(); cam = Camera.main; }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out var hit, 200f, groundMask))
                agent.SetDestination(hit.point);
        }
    }
}
