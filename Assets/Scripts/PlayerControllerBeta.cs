﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBeta : MonoBehaviour
{
    // Attributes
    private float speed;
    private float acceleration = 100f;
    private float max_run_speed = 12.0f;
    private float gravity = 45.0f;
    private float slope_force = 10.0f;
    private float jump_height = 2.5f;
    private float look_sensitivity = 2.0f;
    private float current_speed_x, current_speed_z;

    // Dashing
    private DateTimeOffset lastDash = DateTime.UtcNow;
    private double dashCooldown = .5 * 10000000; //first number is in SECONDS, operator converts to ticks
    private long chainWindow = (long)(.25 * 10000000);
    private long failChainWindow = (long)(.25 * 10000000);
    private enum DashStates {canDash, dashing, inWindow, lockedOut};
    private DashStates dashState = DashStates.canDash;

    // Input
    private float look_input;
    private float horizontal_input, vertical_input;
    
    // Parameters
    private float on_slope_height = 0.45f;
    private float ground_distance;
    private float ground_angle;
    private float slope_distance;
    private float angle;

    // Physics
    private Vector3 move_direction;
    private Ray ground_ray;
    private Ray slope_ray;
    private RaycastHit ground_ray_hit;
    private RaycastHit slope_ray_hit;
    private Vector3 direction;
    private Vector3 flat_direction;
    
    // Components
    CharacterController controller;
    Transform slope_transform;

	private void Start ()
    {
        controller = GetComponent<CharacterController>();
        slope_transform = transform.GetChild(0);
        ground_ray = new Ray(transform.position, -transform.up);
        slope_ray = new Ray(slope_transform.position, -slope_transform.up);
        //Debug.Log("ground_angle: " + ground_angle + " controller: " + controller);
	}

    private void Update()
    {
        direction = transform.position - ground_ray.origin;
        flat_direction = new Vector3(direction.x, 0, direction.z);

        CalculateGroundRay();
        
        slope_transform.rotation = Quaternion.FromToRotation(Vector3.up, ground_ray_hit.normal) * transform.rotation; // Rotate slope ray based on angle beneath player

        CalculateSlopeRay();
        
        angle = Vector3.SignedAngle(direction, flat_direction, Vector3.Cross(transform.up, flat_direction));
        //Debug.Log("angle: " + angle);
        
        UpdateMaxSpeed();
        //Debug.Log("IsRunning(): " + IsRunning() + " IsSliding(): " + IsSliding() + " IsJumping(): " + IsJumping());
        //Debug.Log("speed:" + speed + " increase: " + (speed - speed * 0.99f) + " direction: " + direction + " angle: " + angle);
        
        current_speed_x = HorizontalInput();
        current_speed_z = VerticalInput();
        //Debug.Log("current_speed_x: " + current_speed_x + " current_speed_z: " + current_speed_z + " max_speed: " + max_speed);
        
        if (OnSlope() || controller.isGrounded)
        {
            move_direction.x = (slope_transform.right.x * current_speed_x) + (slope_transform.forward.x * current_speed_z);
            if(controller.isGrounded)move_direction.y = (slope_transform.right.y * current_speed_x) + (slope_transform.forward.y * current_speed_z);
            move_direction.z = (slope_transform.right.z * current_speed_x) + (slope_transform.forward.z * current_speed_z);
        }
        if(!OnSlope() && !controller.isGrounded)
        {
            move_direction.x = (transform.right.x * current_speed_x) + (transform.forward.x * current_speed_z);
            if(controller.isGrounded)move_direction.y = (transform.right.y * current_speed_x) + (transform.forward.y * current_speed_z);
            move_direction.z = (transform.right.z * current_speed_x) + (transform.forward.z * current_speed_z);
        }

        if (controller.isGrounded)
        {
            move_direction.y = -5f; // Keeps character from poping off of peaks
        }

        if (OnSlope()) // if(OnSlope() && !Dashing())
        {
            move_direction.y = -slope_force * speed; // Holds character to slopes at high speed
        }
       
        if (IsJumping() && Input.GetKey(KeyCode.CapsLock))
        {
            move_direction = new Vector3(move_direction.x, slope_transform.up.y * Jump(), move_direction.z);
        }
        //Debug.Log("velocity: " + controller.velocity + " move_direction.y: " + move_direction.y);

        if (IsJumping() && !Input.GetKey(KeyCode.CapsLock))
        {
            move_direction.y = Jump();
        }

        if ((OnSlope() || controller.isGrounded) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            switch (dashState)
            {
                case DashStates.canDash:
                    dashState = DashStates.dashing;
                    Dash();
                    break;
                case DashStates.inWindow:
                    dashState = DashStates.dashing;
                    Dash();
                    break;
                case DashStates.dashing:
                    dashState = DashStates.lockedOut;
                    UpdateDash();
                    break;
                case DashStates.lockedOut:
                    UpdateDash();
                    break;
                default: break;
            }
        }
   



        move_direction.y -= Gravity();

        if (dashState != DashStates.dashing && dashState != DashStates.inWindow)
        {
            Decelerate();
        }
 

        //Debug.Log("controller.isGrounded: " + controller.isGrounded + " ground_distance: " + ground_distance + " slope_distance: " + slope_distance);
        //Debug.Log("move_direction.y: " + move_direction.y + " transform.position.y: " + transform.position.y);
        DebugRays();

        controller.Move(move_direction * Time.deltaTime);
    }

    private void LateUpdate()
    {
        look_input += Input.GetAxis("Mouse X") * look_sensitivity;
        transform.rotation = Quaternion.Euler(0, look_input, 0);
    }

    private void DebugRays()
    {
        Debug.DrawRay(slope_transform.position, slope_transform.right, Color.red, 2);
        Debug.DrawRay(slope_transform.position, slope_transform.up, Color.green, 2);
        Debug.DrawRay(slope_transform.position, slope_transform.forward, Color.blue, 2);
    }

    #region Physics

    private float Gravity()
    {
        if ((!controller.isGrounded || OnSlope()) && move_direction.y > -20.0f)
        {
            return gravity * Time.deltaTime;
        }
        else
        {
            return 0.0f;
        }
    }

    private void UpdateMaxSpeed()
    {
        if (dashState == DashStates.dashing)
        { 

        }

        if (IsRunning())
        {
            speed = max_run_speed;
        }

        if (IsSliding() && OnSlope())
        {
            speed *= 1.0f - angle * 0.02f * Time.deltaTime;
        }

        if (IsJumping())
        {

        }
    }

    private void Decelerate()
    {
        if (speed > max_run_speed && !OnSlope() && controller.isGrounded)
        {
            speed *= 0.99f;
        }

        if (speed > max_run_speed && !OnSlope() && !controller.isGrounded)
        {
            speed *= 0.995f;
        }
    }

    private void CalculateGroundRay()
    {
        ground_ray.origin = transform.position;
        Physics.Raycast(ground_ray, out ground_ray_hit, Mathf.Infinity);
        ground_distance = Vector3.Distance(transform.position, ground_ray_hit.point);
        ground_angle = Vector3.Angle(ground_ray_hit.normal, Vector3.up);
    }

    private void CalculateSlopeRay()
    {
        slope_ray.origin = slope_transform.position;
        slope_ray.direction = -slope_transform.up;
        Physics.Raycast(slope_ray, out slope_ray_hit, Mathf.Infinity);
        slope_distance = Vector3.Distance(slope_transform.position, slope_ray_hit.point);
    }

    #endregion

    #region Game States

    private bool IsRunning()
    {
        if (controller.isGrounded && !IsSliding() && !IsJumping() && dashState != DashStates.dashing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsSliding()
    {
        if (Input.GetKey(KeyCode.CapsLock))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsJumping()
    {
        if ((OnSlope() || controller.isGrounded) && Input.GetKey(KeyCode.Space))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool OnSlope()
    {
        if (ground_angle != 0 && slope_distance <= on_slope_height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool UpdateDash()
    {
        double debugElapsed = (DateTimeOffset.UtcNow.UtcTicks - lastDash.UtcTicks) / 10000000;
        Debug.Log(debugElapsed);

        if ((DateTimeOffset.UtcNow.UtcTicks - lastDash.UtcTicks) >= dashCooldown)
        {
            lastDash = DateTimeOffset.UtcNow;
            return true;
        }
        else
        {
            Debug.Log("failing cooldown check");
            return false;
        }
    }
    #endregion

    #region Simple Movement

    private float HorizontalInput()
    {
        horizontal_input = Input.GetAxis("Horizontal");

        if (horizontal_input > 0)
        {
            return Mathf.Min(horizontal_input * acceleration, speed);
        }
        if (horizontal_input < 0)
        {
            return Mathf.Max(horizontal_input * acceleration, -speed);
        }
        else
        {
            return horizontal_input;
        }
    }

    private float VerticalInput()
    {
        vertical_input = Input.GetAxis("Vertical");

        if (vertical_input > 0)
        {
            return Mathf.Min(vertical_input * acceleration, speed);
        }
        if (vertical_input < 0)
        {
            return Mathf.Max(vertical_input * acceleration, -speed);
        }
        else
        {
            return vertical_input;
        }
    }

    private float Jump()
    {
        return Mathf.Sqrt(2 * jump_height * gravity);
    }

    #endregion

    #region Advanced Movement

    private void Dash()
    {
        speed = speed * 6.0f;
    }


    #endregion

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), (current_speed_x).ToString());
    }
}
