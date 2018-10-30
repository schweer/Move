using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private float run_speed = 12.0F;
    private float sprint_speed = 18.0f;
    private float dash_distance = 5.0F;
    private float jump_speed = 12.0F;
    private float acceleration = 2.0F;
    private float gravity = 30.0F;

    private bool sprinting = false;
    float t;

    private Vector3 move_direction;
    private Camera player_camera;
    private CharacterController controller;

    private KeyCode forward_key = KeyCode.W;
    private KeyCode back_key = KeyCode.A;
    private KeyCode left_key = KeyCode.S;
    private KeyCode right_key = KeyCode.D;
    private KeyCode sprint_key = KeyCode.LeftShift;
    private KeyCode sprint_lock_key = KeyCode.CapsLock;
    private KeyCode jump_key = KeyCode.Space;
    private KeyCode dash_key = KeyCode.Mouse1;
    private KeyCode quit_key = KeyCode.Escape;

    private void Start()
    {
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(quit_key))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(sprint_lock_key))
        {
            sprinting = !sprinting;
        }

        if (Input.GetKey(sprint_key) || sprinting)
        {
            speed = sprint_speed;
        }
        else
        {
            speed = run_speed;
        }

        if (Input.GetKey(forward_key))
        {
            transform.position += new Vector3(player_camera.transform.forward.x, 0, player_camera.transform.forward.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(back_key))
        {
            transform.position += new Vector3(-player_camera.transform.right.x, 0, -player_camera.transform.right.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(left_key))
        {
            transform.position += new Vector3(-player_camera.transform.forward.x, 0, -player_camera.transform.forward.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(right_key))
        {
            transform.position += new Vector3(player_camera.transform.right.x, 0, player_camera.transform.right.z) * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(dash_key) && Input.GetKey(forward_key) && controller.isGrounded)
        {
            Vector3.Lerp(transform.position, transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * dash_distance, t);
        }

        if (Input.GetKey(jump_key) && controller.isGrounded)
        {
            move_direction.y = jump_speed;
        }

        move_direction.y -= gravity * Time.deltaTime;
        controller.Move(move_direction * Time.deltaTime);
    }
}