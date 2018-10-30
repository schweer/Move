using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed;
    private float run_speed = 12.0F;
    private float jump_speed = 12.0F;
    private float gravity = 10.0F;

    private LayerMask ground_layer;
    private float distance_to_ground;
    
    private Camera player_camera;
    private Rigidbody rigid_body;
    private Collider player_collider;
    
    private KeyCode forward_key = KeyCode.W;
    private KeyCode back_key = KeyCode.A;
    private KeyCode left_key = KeyCode.S;
    private KeyCode right_key = KeyCode.D;
    //private KeyCode sprint_key = KeyCode.LeftShift;
    //private KeyCode sprint_lock_key = KeyCode.CapsLock;
    private KeyCode jump_key = KeyCode.Space;
    //private KeyCode dash_key = KeyCode.Mouse1;
    private KeyCode quit_key = KeyCode.Escape;
    
    private void Start()
    {
        ground_layer = LayerMask.NameToLayer("Ground");
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        rigid_body = GetComponent<Rigidbody>();
        player_collider = GetComponent<Collider>();
        distance_to_ground = player_collider.bounds.extents.y;
       // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(quit_key))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // shooter
        if (Input.GetKey(forward_key))
        {
            
        }
        if (Input.GetKey(back_key))
        {
            
        }
        if (Input.GetKey(left_key))
        {
            
        }
        if (Input.GetKey(right_key))
        {
            
        }
        if (Input.GetKey(jump_key) && IsGrounded())
        {
            
        }

        Debug.Log(IsGrounded());
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distance_to_ground, ~ground_layer);
    }
}