// Assets/AI/Coord/InfluenceMap.cs
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{
    public Vector2 size = new Vector2(60, 60);   // meters
    public float cell = 2f;                     // cell size
    public float decayPerSecond = 0.9f;         // 10%/s decay

    float[,] grid; int nx, nz; Vector3 origin;

    void Awake()
    {
        nx = Mathf.CeilToInt(size.x / cell);
        nz = Mathf.CeilToInt(size.y / cell);
        grid = new float[nx, nz];
        origin = transform.position - new Vector3(size.x * 0.5f, 0, size.y * 0.5f);
    }

    void Update()
    {
        float k = Mathf.Clamp01(1f - decayPerSecond * Time.deltaTime);
        for (int x = 0; x < nx; x++) for (int z = 0; z < nz; z++) grid[x, z] *= k;
    }

    public void AddPulse(Vector3 pos, float strength)
    {
        ToCell(pos, out int x, out int z);
        if (Inside(x, z)) grid[x, z] += strength;
    }

    public Vector3 SampleGradient(Vector3 pos)
    {
        ToCell(pos, out int x, out int z);
        if (!Inside(x, z)) return Vector3.zero;
        float cx = (Value(x + 1, z) - Value(x - 1, z)) * 0.5f;
        float cz = (Value(x, z + 1) - Value(x, z - 1)) * 0.5f;
        return new Vector3(cx, 0, cz);
    }

    float Value(int x, int z) { return Inside(x, z) ? grid[x, z] : 0f; }
    bool Inside(int x, int z) { return x >= 0 && x < nx && z >= 0 && z < nz; }

    void ToCell(Vector3 pos, out int x, out int z)
    {
        Vector3 p = pos - origin;
        x = Mathf.FloorToInt(p.x / cell);
        z = Mathf.FloorToInt(p.z / cell);
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        for (int x = 0; x < nx; x++) for (int z = 0; z < nz; z++)
            {
                float v = Mathf.Clamp01(grid[x, z]);
                if (v <= 0.01f) continue;
                Vector3 c = origin + new Vector3((x + 0.5f) * cell, 0, (z + 0.5f) * cell);
                Gizmos.color = new Color(1f, 0f, 0f, v * 0.4f);
                Gizmos.DrawCube(c + Vector3.up * 0.01f, new Vector3(cell, 0.02f, cell));
            }
    }
}
