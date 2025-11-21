using UnityEngine;

public class ZombieCatch : MonoBehaviour
{
    public Transform player;   // drag Player
    public float catchRange = 1.2f;
    public GameUI gameUI;      // drag Canvas GameUI
    bool caught;

    void Awake()
    {
        if (!player)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
        }
        if (!gameUI) gameUI = FindObjectOfType<GameUI>();
        if (!gameUI) Debug.LogWarning("ZombieCatch: GameUI not found; catch messages will not display.");
    }

    void Update()
    {
        if (caught) return;
        if (!player) return;
        if (Vector3.Distance(transform.position, player.position) <= catchRange)
        {
            if (gameUI) gameUI.ShowMessage("Caught");
            Time.timeScale = 0f;
            caught = true;
        }
    }
}
