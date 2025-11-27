
using System.Collections.Generic;
using UnityEngine;
public struct NoiseEvent { public Vector3 pos; public float intensity; public float radius; public float time; public string type; }
public static class NoiseEventBus
{
    static readonly List<NoiseEvent> list = new();
    static float lastTime;

    static void EnsureFreshTime()
    {
        
        
        if (Time.time < lastTime) list.Clear();
        lastTime = Time.time;
    }
    public static void Raise(Vector3 pos, float intensity, float radius, string type = "generic")
    {
        EnsureFreshTime();
        list.Add(new NoiseEvent { pos = pos, intensity = intensity, radius = radius, time = Time.time, type = type });
    }
    public static bool TryGetStrongestNear(Vector3 p, float maxAge, out NoiseEvent e, float radiusMultiplier = 1f)
    {
        EnsureFreshTime();
        e = default;
        float bestScore = -1f;
        float bestTime = -1f;
        for (int i = list.Count - 1; i >= 0; --i)
        {
            var ev = list[i];
            if (Time.time - ev.time > maxAge) { list.RemoveAt(i); continue; }
            float d = Vector3.Distance(p, ev.pos);
            if (d > ev.radius * Mathf.Max(0.01f, radiusMultiplier)) continue;
            float s = ev.intensity / (1f + d);
            
            if (ev.time > bestTime || (Mathf.Approximately(ev.time, bestTime) && s > bestScore))
            {
                bestTime = ev.time;
                bestScore = s;
                e = ev;
            }
        }
        return bestScore >= 0;
    }
}
