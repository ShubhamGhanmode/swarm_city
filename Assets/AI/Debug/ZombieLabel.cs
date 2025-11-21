using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class ZombieLabel : MonoBehaviour
{
    public ZombieBrain brain;     // drag from same zombie (autofilled if left empty)
    public TextMeshProUGUI text;  // drag StateText
    public Blackboard bb;         // drag from same zombie (autofilled if left empty)
    public bool faceCamera = true;

    void Reset() { AutoWire(); }
    void Awake() { AutoWire(); }

    void LateUpdate()
    {
        if (!text) { AutoWire(); if (!text) return; }
        if (!brain || !bb) AutoWire();
        if (!brain || !bb) return;

        string seen = bb.lastSeen.HasValue ? "Seen" : "-";
        string heard = bb.lastHeard.HasValue ? "Heard" : "-";
        text.text = $"{brain.CurrentStateName}\nSusp {bb.suspicion:0.00}\n{seen}/{heard}";

        if (faceCamera && Camera.main)
        {
            var t = text.transform.parent ? text.transform.parent : text.transform;
            t.forward = Camera.main.transform.forward;
        }
    }

    void AutoWire()
    {
        if (!brain) brain = GetComponentInParent<ZombieBrain>();
        if (!bb) bb = GetComponentInParent<Blackboard>();
        if (!text) text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
