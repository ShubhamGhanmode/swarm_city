// /AI/Core/Blackboard.cs
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public Vector3? lastHeard;
    public Vector3? lastSeen;
    public float suspicion;   // 0..1

    // Ensure the AI always starts "calm" and without stale data
    void OnEnable()
    {
        lastHeard = null;
        lastSeen = null;
        suspicion = 0f;
    }
}
