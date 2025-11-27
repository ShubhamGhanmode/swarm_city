using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public PlayerControllerFP player;     
    public TextMeshProUGUI label;         

    void Update()
    {
        if (!player || !label) return;
        string stance = player.IsCrouched ? "Crouching" : "Standing";
        string grounded = player.IsGrounded ? "Grounded" : "Air";
        label.text = $"State: {stance} | {grounded}";
    }
}
