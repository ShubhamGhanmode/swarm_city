using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance { get; private set; }

    public TextMeshProUGUI objectiveText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!objectiveText)
            objectiveText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetObjective(string text)
    {
        if (!objectiveText) return;
        objectiveText.text = "Objective: " + text;
    }
}
