using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBeta : MonoBehaviour
{
    // Attributes
    private float acceleration = 20.0f;
    private float max_run_speed = 8.0f;
    private float gravity = 30.0f;
    private float slope_force = 25.0f;
    private float jump_height = 2.5f;
    private float look_sensitivity = 2.0f;

    // Input
    private float look_input;
    private float horizontal_input, vertical_input;
    private float current_speed_x, current_speed_z;
    private float max_speed;

    // Parameters
    private float on_slope_height = 0.45f;
    private float ground_distance;
    private float ground_angle;
    private float slope_distance;

    // Physics
    private Vector3 move_direction;
    private Ray ground_ray;
    private Ray slope_ray;
    private RaycastHit ground_ray_hit;
    private RaycastHit slope_ray_hit;

    // Components
    CharacterController controller;
    Transform slope_transform;

    Vector3 direction;
    float angle;

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
        direction.y = 0.0f;
        //Debug.Log("direction: " + direction);

        ground_ray.origin = transform.position;
        Physics.Raycast(ground_ray, out ground_ray_hit, Mathf.Infinity);
        ground_distance = Vector3.Distance(transform.position, ground_ray_hit.point);
        ground_angle = Vector3.Angle(ground_ray_hit.normal, Vector3.up);
        
        slope_transform.rotation = Quaternion.FromToRotation(Vector3.up, ground_ray_hit.normal) * transform.rotation; // Rotate slope ray based on angle beneath player

        slope_ray.origin = slope_transform.position;
        slope_ray.direction = -slope_transform.up;
        Physics.Raycast(slope_ray, out slope_ray_hit, Mathf.Infinity);
        slope_distance = Vector3.Distance(slope_transform.position, slope_ray_hit.point);
        
        angle = Vector3.SignedAngle(direction, slope_transform.forward, -transform.right);
        Debug.Log("angle: " + angle);

        if (OnSlope() && angle < 0 && Input.GetKey(KeyCode.CapsLock))
        {
            max_speed = Mathf.Infinity;
        }
        else
        {
            max_speed = max_run_speed;
        }

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
            move_direction.y = -5f;
        }

        if (current_speed_x <= max_speed && current_speed_z <= max_speed && OnSlope())
        {
            move_direction.y = -slope_force;
        }

        if (IsJumping())
        {
            move_direction.y = Jump();
        }

        move_direction.y -= Gravity();

        //Debug.Log("controller.isGrounded: " + controller.isGrounded + " ground_distance: " + ground_distance + " slope_distance: " + slope_distance);
        //Debug.Log("move_direction.y: " + move_direction.y + " transform.position.y: " + transform.position.y);
        Debug.DrawRay(slope_transform.position, slope_transform.right, Color.red, 2);
        Debug.DrawRay(slope_transform.position, slope_transform.up, Color.green, 2);
        Debug.DrawRay(slope_transform.position, slope_transform.forward, Color.blue, 2);

        controller.Move(move_direction * Time.deltaTime);
    }

    private void LateUpdate()
    {
        look_input += Input.GetAxis("Mouse X") * look_sensitivity;
        transform.rotation = Quaternion.Euler(0, look_input, 0);
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

    private float Gravity()
    {
        if (!controller.isGrounded && move_direction.y > -20.0f)
        {
            return gravity * Time.deltaTime;
        }
        else
        {
            return 0.0f;
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

    private float Jump()
    {
        return Mathf.Sqrt(2 * jump_height * gravity);
    }

    private float HorizontalInput()
    {
        horizontal_input = Input.GetAxis("Horizontal");
        
        if (horizontal_input > 0)
        {
            return Mathf.Min(horizontal_input * acceleration, max_speed);
        }
        if (horizontal_input < 0)
        {
            return Mathf.Max(horizontal_input * acceleration, -max_speed);
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
            return Mathf.Min(vertical_input * acceleration, max_speed);
        }
        if (vertical_input < 0)
        {
            return Mathf.Max(vertical_input * acceleration, -max_speed);
        }
        else
        {
            return vertical_input;
        }
    }
}
