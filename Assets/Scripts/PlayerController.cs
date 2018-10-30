using UnityEngine;
using System.Collections;

// The GameObject is made to bounce using the space key.
// Also the GameOject can be moved forward/backward and left/right.
// Add a Quad to the scene so this GameObject can collider with a floor.

public class PlayerController : MonoBehaviour
{
    public float current_speed_x = 0.0f;
    public float current_speed_z = 0.0f;
    public float acceleration = 100.0f;
    public float max_speed = 30.0f;
    public float jump_speed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 move_direction = Vector3.zero;
    private CharacterController controller;
    private float horizontal_input, vertical_input;

    private KeyCode left_key = KeyCode.A;
    private KeyCode right_key = KeyCode.D;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");

        if (horizontal_input > 0)
        {
            current_speed_x = Mathf.Min(horizontal_input * acceleration, max_speed);
        }
        if (vertical_input > 0)
        {
            current_speed_z = Mathf.Min(vertical_input * acceleration, max_speed);
        }
        if (horizontal_input < 0)
        {
            current_speed_x = Mathf.Max(horizontal_input * acceleration, -max_speed);
        }
        if (vertical_input < 0)
        {
            current_speed_z = Mathf.Max(vertical_input * acceleration, -max_speed);
        }
        if (horizontal_input == 0)
        {
            current_speed_x = horizontal_input;
        }
        if (vertical_input == 0)
        {
            current_speed_z = vertical_input;
        }

        move_direction.x = current_speed_x;
        move_direction.z = current_speed_z;

        if (Input.GetButton("Jump") && controller.isGrounded)
        {
            move_direction.y = jump_speed;
        }

        move_direction.y -= (gravity * Time.deltaTime);
        
        controller.Move(move_direction * Time.deltaTime);
    }
}