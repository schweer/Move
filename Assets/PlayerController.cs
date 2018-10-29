﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private float run_speed = 12.0F;
    private float sprint_speed = 18.0f;
    private float jump_speed = 12.0F;
    private float gravity = 30.0F;

    private bool sprinting = false;

    private Vector3 move_direction = Vector3.zero;
    private Camera player_camera;
    private CharacterController controller;

    public KeyCode forward_key = KeyCode.W;
    public KeyCode back_key = KeyCode.A;
    public KeyCode left_key = KeyCode.S;
    public KeyCode right_key = KeyCode.D;
    public KeyCode dash_key = KeyCode.LeftShift;
    public KeyCode dash_lock_key = KeyCode.CapsLock;
    public KeyCode jump_key = KeyCode.Space;

    private void Start()
    {
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(dash_lock_key))
        {
            sprinting = !sprinting;
        }

        if (Input.GetKey(dash_key) || sprinting)
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

        if (controller.isGrounded)
        {
            if (Input.GetKey(jump_key) && controller.isGrounded)
            {
                move_direction.y = jump_speed;
            }
        }

        move_direction.y -= gravity * Time.deltaTime;
        controller.Move(move_direction * Time.deltaTime);
    }
}