// /AI/Perception/NoiseEventBus.cs
using System.Collections.Generic;
using UnityEngine;
public struct NoiseEvent { public Vector3 pos; public float intensity; public float radius; public float time; public string type; }
public static class NoiseEventBus
{
    static readonly List<NoiseEvent> list = new();
    static float lastTime;

    static void EnsureFreshTime()
    {
        // When "reload domain" is disabled in Unity, static lists survive play-mode restarts.
        // If Time.time jumps backwards we clear stale events so zombies don't react to old noise.
        if (Time.time < lastTime) list.Clear();
        lastTime = Time.time;
    }
    public static void Raise(Vector3 pos, float intensity, float radius, string type = "generic")
    {
        EnsureFreshTime();
        list.Add(new NoiseEvent { pos = pos, intensity = intensity, radius = radius, time = Time.time, type = type });
    }
    public static bool TryGetStrongestNear(Vector3 p, float maxAge, out NoiseEvent e)
    {
        EnsureFreshTime();
        e = default; float best = -1f;
        for (int i = list.Count - 1; i >= 0; --i)
        {
            var ev = list[i];
            if (Time.time - ev.time > maxAge) { list.RemoveAt(i); continue; }
            float d = Vector3.Distance(p, ev.pos); if (d > ev.radius) continue;
            float s = ev.intensity / (1f + d); if (s > best) { best = s; e = ev; }
        }
        return best >= 0;
    }
}
