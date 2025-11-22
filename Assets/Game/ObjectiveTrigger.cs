using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveTrigger : MonoBehaviour
{
    [TextArea]
    public string newObjectiveText = "New objective";
    public bool destroyOnEnter = true;

    public GameUI gameUI;

    void Awake()
    {
        if (!gameUI) gameUI = FindObjectOfType<GameUI>();
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameUI)
            gameUI.SetObjective(newObjectiveText);

        if (destroyOnEnter)
            Destroy(gameObject);
    }
}
