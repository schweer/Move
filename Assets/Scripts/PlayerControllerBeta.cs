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
    private float on_slope_height = 0.5f;
    private float close_to_ground_force = 25.0f;

    Vector3 move_direction;
    Ray ground_ray;
    RaycastHit ground_ray_hit;
    float angle;

    //Test Comment - Chance

    CharacterController controller;
    Transform ground_angle;

	private void Start ()
    {
        controller = GetComponent<CharacterController>();
        ground_angle = transform.GetChild(0);
        ground_ray = new Ray(transform.position, -transform.up);
        //Debug.Log("ground_angle: " + ground_angle + " controller: " + controller);
	}

    private void Update()
    {
        ground_ray.origin = transform.position;
        Physics.Raycast(ground_ray, out ground_ray_hit, Mathf.Infinity); // More granularity in slope movement.
        //Physics.SphereCast(ground_ray, 0.5f, out ground_ray_hit, Mathf.Infinity); // Holds on to slopes better.

        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");

        ground_angle.rotation = Quaternion.FromToRotation(Vector3.up, ground_ray_hit.normal) * transform.rotation;
        

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

        angle = Vector3.Angle(ground_ray_hit.normal, Vector3.up);

        move_direction.x = (ground_angle.right.x * current_speed_x) + (ground_angle.forward.x * current_speed_z);
        if(controller.isGrounded || (Vector3.Distance(transform.position, ground_ray_hit.point) <= on_slope_height)) move_direction.y = (ground_angle.right.y * current_speed_x) + (ground_angle.forward.y * current_speed_z);
        move_direction.z = (ground_angle.right.z * current_speed_x) + (ground_angle.forward.z * current_speed_z);
        
        if (controller.isGrounded)
        {
            move_direction.y = -5f;
        }

        //jump at ground_angle

        if (current_speed_x <= max_speed && current_speed_z <= max_speed && angle != 0 && Vector3.Distance(transform.position, ground_ray_hit.point) < on_slope_height)
        {
            move_direction.y = -close_to_ground_force;
        }

        if (!controller.isGrounded && Vector3.Distance(transform.position, ground_ray_hit.point) > on_slope_height)
        {
            move_direction.y -= gravity * Time.deltaTime;
        }
        
        //Debug.Log("controller.isGrounded: " + controller.isGrounded + " move_direction.y: " + move_direction.y);
        Debug.Log("angle: " + angle);
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
