using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameUI gameUI; // drag reference
    [TextArea]
    public string goalMessage = "You escaped!";

    bool triggered;

    void Awake()
    {
        if (!gameUI) gameUI = FindObjectOfType<GameUI>();
        if (!gameUI) Debug.LogWarning("GoalTrigger: GameUI not found; goal messages will not display.");
    }
    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        if (gameUI) gameUI.ShowMessage(goalMessage);
        Time.timeScale = 0f; // pause
    }
}
