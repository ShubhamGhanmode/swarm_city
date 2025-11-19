using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    CharacterController controller;

    void Awake() { controller = GetComponent<CharacterController>(); }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v).normalized;

        if (move.magnitude >= 0.1f)
            controller.Move(move * speed * Time.deltaTime);
    }
}
