using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBeta : MonoBehaviour {
    
    private float look_input;
    private float look_sensitivity = 2.0f;
    private float horizontal_input, vertical_input;
    private float current_speed_x, current_speed_z;
    private float acceleration = 100.0f;
    private float max_speed = 8.0f;
    private float gravity = 30.0f;
    private float on_slope_height = 0.45f;
    private float ground_distance;
    private float slope_distance;
    private bool on_slope = false;
    private float slope_force = 25.0f;
    private float jump_speed = 10.0f;

    bool jumping = false;

    Vector3 move_direction;
    Ray ground_ray;
    RaycastHit ground_ray_hit;
    Ray slope_ray;
    RaycastHit slope_ray_hit;
    float angle;

    CharacterController controller;
    Transform ground_angle;

	private void Start ()
    {
        controller = GetComponent<CharacterController>();
        ground_angle = transform.GetChild(0);
        ground_ray = new Ray(transform.position, -transform.up);
        slope_ray = new Ray(ground_angle.position, -ground_angle.up);
        //Debug.Log("ground_angle: " + ground_angle + " controller: " + controller);
	}

    private void Update()
    {
        ground_ray.origin = transform.position;
        Physics.Raycast(ground_ray, out ground_ray_hit, Mathf.Infinity); // More granularity in slope movement.
        //Physics.SphereCast(ground_ray, 0.5f, out ground_ray_hit, Mathf.Infinity); // Holds on to slopes better.
        Physics.Raycast(slope_ray, out slope_ray_hit, Mathf.Infinity);

        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");

        angle = Vector3.Angle(ground_ray_hit.normal, Vector3.up);
        
        ground_distance = Vector3.Distance(transform.position, ground_ray_hit.point);

        ground_angle.rotation = Quaternion.FromToRotation(Vector3.up, ground_ray_hit.normal) * transform.rotation;
        slope_ray.origin = ground_angle.position;
        slope_ray.direction = -ground_angle.up;
        slope_distance = Vector3.Distance(ground_angle.position, slope_ray_hit.point);

        if (angle != 0 && slope_distance <= on_slope_height)
        {
            on_slope = true;
        }
        else
        {
            on_slope = false;
        }

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

        /*
        move_direction.x = (ground_angle.right.x * current_speed_x) + (ground_angle.forward.x * current_speed_z);
        if(controller.isGrounded || on_slope) move_direction.y = (ground_angle.right.y * current_speed_x) + (ground_angle.forward.y * current_speed_z);
        move_direction.z = (ground_angle.right.z * current_speed_x) + (ground_angle.forward.z * current_speed_z);
        */

        if (on_slope || controller.isGrounded)
        {
            move_direction.x = (ground_angle.right.x * current_speed_x) + (ground_angle.forward.x * current_speed_z);
            if(controller.isGrounded)move_direction.y = (ground_angle.right.y * current_speed_x) + (ground_angle.forward.y * current_speed_z);
            move_direction.z = (ground_angle.right.z * current_speed_x) + (ground_angle.forward.z * current_speed_z);
        }
        if(!on_slope && !controller.isGrounded)
        {
            move_direction.x = (transform.right.x * current_speed_x) + (transform.forward.x * current_speed_z);
            if(controller.isGrounded)move_direction.y = (transform.right.y * current_speed_x) + (transform.forward.y * current_speed_z);
            move_direction.z = (transform.right.z * current_speed_x) + (transform.forward.z * current_speed_z);
        }
        
        if (controller.isGrounded)
        {
            move_direction.y = -5f;
        }

        //jump at ground_angle if sliding; probably have to remove slope force
        if ((on_slope || controller.isGrounded) && Input.GetKey(KeyCode.Space))
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }

        if (current_speed_x <= max_speed && current_speed_z <= max_speed && angle != 0 && on_slope)
        {
            move_direction.y = -slope_force;
        }

        if (jumping)
        {
            move_direction.y = jump_speed;
        }

        if (!controller.isGrounded)
        {
            move_direction.y -= gravity * Time.deltaTime;
        }
        
        Debug.Log("controller.isGrounded: " + controller.isGrounded + " ground_distance: " + ground_distance + " slope_distance: " + slope_distance);
        //Debug.Log("angle: " + angle);
        Debug.DrawRay(ground_angle.position, ground_angle.right, Color.red, 2);
        Debug.DrawRay(ground_angle.position, ground_angle.up, Color.green, 2);
        Debug.DrawRay(ground_angle.position, ground_angle.forward, Color.blue, 2);

        controller.Move(move_direction * Time.deltaTime);
    }

    private void LateUpdate()
    {
        look_input += Input.GetAxis("Mouse X") * look_sensitivity;
        transform.rotation = Quaternion.Euler(0, look_input, 0);
    }
}
