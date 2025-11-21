using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public PlayerControllerFP player;     // drag Player here
    public TextMeshProUGUI label;         // drag PlayerStatusText here

    void Update()
    {
        if (!player || !label) return;
        string stance = player.IsCrouched ? "Crouching" : "Standing";
        string grounded = player.IsGrounded ? "Grounded" : "Air";
        label.text = $"State: {stance} | {grounded}";
    }
}
