using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0F;
    public float jump_speed = 8.0F;
    public float gravity = 20.0F;

    private Vector3 move_direction = Vector3.zero;
    private Camera player_camera;
    private CharacterController controller;

    public KeyCode forward_key = KeyCode.W;
    public KeyCode back_key = KeyCode.A;
    public KeyCode left_key = KeyCode.S;
    public KeyCode right_key = KeyCode.D;
    public KeyCode jump_key = KeyCode.Space;

    private void Start()
    {
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

        if (Input.GetKey(forward_key))
        {
            transform.position = transform.position + new Vector3(player_camera.transform.forward.x, 0, player_camera.transform.forward.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(back_key))
        {
            transform.position = transform.position + new Vector3(-player_camera.transform.right.x, 0, -player_camera.transform.right.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(left_key))
        {
            transform.position = transform.position + new Vector3(-player_camera.transform.forward.x, 0, -player_camera.transform.forward.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(right_key))
        {
            transform.position = transform.position + new Vector3(player_camera.transform.right.x, 0, player_camera.transform.right.z) * speed * Time.deltaTime;
        }

        if (controller.isGrounded)
        {
            if (Input.GetKey(jump_key))
            {
                move_direction.y = jump_speed;
            }
        }

        move_direction.y -= gravity * Time.deltaTime;
        controller.Move(move_direction * Time.deltaTime);
    }
}