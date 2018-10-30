using UnityEngine;
using System.Collections;

//test

public class PlayerController : MonoBehaviour
{
    public float current_speed_x = 0.0f;
    public float current_speed_z = 0.0f;
    public float ground_acceleration = 100.0f;
    public float aerial_acceleration = 50.0f;
    public float acceleration = 100.0f;
    public float max_speed = 20.0f;
    public float jump_speed = 20.0f;
    public float gravity = 80.0f;
    public float dash_distance = 20.0f;
    public Vector3 drag = new Vector3(0.2f, 0.2f, 0.2f);

    private Vector3 move_direction = Vector3.zero;
    private CharacterController controller;
    private float horizontal_input, vertical_input;
    
    private KeyCode dash_key = KeyCode.Mouse1;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        if (controller.isGrounded)
        {
            acceleration = ground_acceleration;
        }
        else
        {
            acceleration = aerial_acceleration;
        }

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

        /*
        if (Input.GetKey(dash_key) && move_direction.x > 0)
        {
            move_direction.x += dash_distance;
        }
        if (Input.GetKey(dash_key) && move_direction.x < 0)
        {
            move_direction.x -= dash_distance;
        }
        if (Input.GetKey(dash_key) && move_direction.z > 0)
        {
            move_direction.z += dash_distance;
        }
        if (Input.GetKey(dash_key) && move_direction.z < 0)
        {
            move_direction.z -= dash_distance;
        }
        */

        if (controller.isGrounded && move_direction.y < 0)
        {
            move_direction.y = 0f; // Keeps alternating between grounded and not grounded
        }

        if (controller.isGrounded && Input.GetButton("Jump"))
        {
            move_direction.y = jump_speed;
        }

        if (!controller.isGrounded)
        {
            move_direction.y -= (gravity * Time.deltaTime);
        }

        //Debug.Log("move_direction.x, move_direction.z: " + move_direction.x + ", " + move_direction.z);
        //Debug.Log("grounded: " + controller.isGrounded + " magnitude: " + move_direction.magnitude);
        controller.Move(move_direction * Time.deltaTime);

        move_direction.x /= 1 + drag.x * Time.deltaTime;
        move_direction.y /= 1 + drag.y * Time.deltaTime;
        move_direction.z /= 1 + drag.z * Time.deltaTime;
    }
}