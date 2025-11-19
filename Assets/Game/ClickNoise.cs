// /Game/ClickNoise.cs
using UnityEngine;
public class ClickNoise : MonoBehaviour
{
    public LayerMask groundMask; public float intensity = 12f; public float radius = 25f;
    Camera cam;
    void Awake() { cam = Camera.main; }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 200f, groundMask))
                NoiseEventBus.Raise(hit.point, intensity, radius, "click");
        }
    }
}
