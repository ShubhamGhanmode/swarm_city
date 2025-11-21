using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameUI gameUI; // drag reference
    void Awake()
    {
        if (!gameUI) gameUI = FindObjectOfType<GameUI>();
        if (!gameUI) Debug.LogWarning("GoalTrigger: GameUI not found; goal messages will not display.");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameUI) gameUI.ShowMessage("You escaped!");
            Time.timeScale = 0f; // pause
        }
    }
}
