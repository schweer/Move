using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0F;
    public float jump_speed = 8.0F;
    public float gravity = 20.0F;
    public Vector3 move_direction = Vector3.zero;
    public Camera player_camera;
    public CharacterController controller;

    private void Start()
    {
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + new Vector3(player_camera.transform.forward.x, 0, player_camera.transform.forward.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + new Vector3(-player_camera.transform.right.x, 0, -player_camera.transform.right.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + new Vector3(-player_camera.transform.forward.x, 0, -player_camera.transform.forward.z) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + new Vector3(player_camera.transform.right.x, 0, player_camera.transform.right.z) * speed * Time.deltaTime;
        }

        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                move_direction.y = jump_speed;
            }
        }

        move_direction.y -= gravity * Time.deltaTime;
        controller.Move(move_direction * Time.deltaTime);
    }
}