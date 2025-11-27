using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    public GameObject zombiePrefab;
    public int initialCount = 3;
    public float spawnRadius = 20f;
    public float navSampleRadius = 2f;

    [Header("Timed Spawns (Optional)")]
    public bool enableTimedSpawns = false;
    public float spawnInterval = 10f;
    public int maxAlive = 15;

    
    readonly List<GameObject> alive = new();

    void Start()
    {
        if (!zombiePrefab) return;

        
        Spawn(initialCount);

        
        if (enableTimedSpawns)
            StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (enableTimedSpawns)
        {
            yield return new WaitForSeconds(spawnInterval);

            
            if (alive.Count < maxAlive)
                Spawn(1);
        }
    }

    void Spawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (TrySamplePosition(out var pos))
            {
                
                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                var z = Instantiate(zombiePrefab, pos, rot);
                alive.Add(z);
            }
        }
    }

    bool TrySamplePosition(out Vector3 position)
    {
        Vector3 center = transform.position;

        for (int i = 0; i < 8; i++) 
        {
            var offset = Random.insideUnitCircle * spawnRadius;
            var candidate = center + new Vector3(offset.x, 0f, offset.y);

            if (NavMesh.SamplePosition(candidate, out var hit, navSampleRadius, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }
        }

        
        position = center;
        return false;
    }
}
