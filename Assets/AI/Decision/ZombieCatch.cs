using UnityEngine;

public class ZombieCatch : MonoBehaviour
{
    public Transform player;   
    public float catchRange = 1.2f;
    public float verticalTolerance = 0.6f;
    public GameUI gameUI;      
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
        Vector3 selfPos = transform.position;
        Vector3 playerPos = player.position;
        float verticalGap = Mathf.Abs(selfPos.y - playerPos.y);
        if (verticalGap > verticalTolerance) return;
        Vector2 planar = new Vector2(selfPos.x - playerPos.x, selfPos.z - playerPos.z);
        if (planar.magnitude <= catchRange)
        {
            if (gameUI) gameUI.ShowMessage("Caught");
            Time.timeScale = 0f;
            caught = true;
        }
    }
}
