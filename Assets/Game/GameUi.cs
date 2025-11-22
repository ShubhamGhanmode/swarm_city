using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Center message (win/lose/pause)")]
    public TextMeshProUGUI centerMessage; // drag CenterMessage

    [Header("Optional objective text (top left)")]
    public TextMeshProUGUI objectiveText; // drag ObjectiveText (if you create it)

    void Reset() { AutoWire(); }
    void Awake() { AutoWire(); }

    // Center message + restart (same behaviour as before)
    public void ShowMessage(string msg)
    {
        if (!centerMessage)
        {
            AutoWire();
            if (!centerMessage)
            {
                Debug.LogWarning("GameUI: centerMessage not assigned and could not auto-find.");
                return;
            }
        }
        centerMessage.gameObject.SetActive(true);
        centerMessage.text = msg + "\nPress R to restart";
    }

    public void HideMessage()
    {
        if (centerMessage)
            centerMessage.gameObject.SetActive(false);
    }

    // NEW: set objective text, if one is assigned
    public void SetObjective(string text)
    {
        if (objectiveText)
            objectiveText.text = "Objective: " + text;
    }

    void Update()
    {
        if (centerMessage && centerMessage.gameObject.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
              UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    void AutoWire()
    {
        if (!centerMessage)
            centerMessage = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
    }
}
