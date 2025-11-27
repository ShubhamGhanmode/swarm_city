
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public Vector3? lastHeard;
    public Vector3? lastSeen;
    public float suspicion;   

    
    void OnEnable()
    {
        lastHeard = null;
        lastSeen = null;
        suspicion = 0f;
    }
}
