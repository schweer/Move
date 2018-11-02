using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBeta : MonoBehaviour {
    
    private float look_input;
    private float look_sensitivity = 2f;
    private float horizontal_input, vertical_input;
    private float current_speed_x, current_speed_z;
    private float acceleration = 100;
    private float max_speed = 8;
    private float gravity = 30;
    private float step_height = 0.5f;

    Vector3 move_direction;
    Ray ground_ray;
    RaycastHit ground_ray_hit;
    Ray angle_ray;
    RaycastHit angle_ray_hit;
    RaycastHit hit;
    float angle;

    CharacterController controller;
    Transform ground_angle;

	private void Start ()
    {
        controller = GetComponent<CharacterController>();
        ground_angle = transform.GetChild(0);
        ground_ray = new Ray(transform.position, -transform.up);
        angle_ray = new Ray(transform.position, -transform.up);
        //Debug.Log("ground_angle: " + ground_angle + " controller: " + controller);
	}

    private void Update()
    {
        ground_ray.origin = transform.position;
        angle_ray.origin = transform.position;
        Physics.Raycast(angle_ray, out angle_ray_hit, Mathf.Infinity); // More granularity in slope movement.
        Physics.SphereCast(ground_ray, 0.5f, out ground_ray_hit, Mathf.Infinity); // Holds on to slopes better.
        angle = Vector3.Angle(angle_ray_hit.normal, Vector3.up);

        if (angle <= 45)
        {
            hit = angle_ray_hit;
        }
        else
        {
            hit = ground_ray_hit;
        }

        ground_angle.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;

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

        move_direction.x = (ground_angle.right.x * current_speed_x) + (ground_angle.forward.x * current_speed_z);
        if(controller.isGrounded || (Vector3.Distance(transform.position, hit.point) <= step_height)) move_direction.y = (ground_angle.right.y * current_speed_x) + (ground_angle.forward.y * current_speed_z);
        move_direction.z = (ground_angle.right.z * current_speed_x) + (ground_angle.forward.z * current_speed_z);
        
        if (controller.isGrounded)
        {
            move_direction.y = -0.5f;
        }

        if (!controller.isGrounded && Vector3.Distance(transform.position, hit.point) > step_height)
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
