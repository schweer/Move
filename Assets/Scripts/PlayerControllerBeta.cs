using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBeta : MonoBehaviour {
    
    private float look_input;
    private float look_sensitivity = 2f;
    private float horizontal_input, vertical_input;
    private float current_speed_x, current_speed_z;
    private float acceleration = 100;
    private float max_speed = 15;

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
        ground_ray.direction = -transform.up;
        //Debug.Log("ground_angle: " + ground_angle + " controller: " + controller);
	}
	
	private void Update ()
    {
        ground_ray.origin = transform.position;
        //Physics.Raycast(ground_ray, out ground_ray_hit, Mathf.Infinity);
        Physics.SphereCast(ground_ray, 0.2f, out ground_ray_hit, Mathf.Infinity);

        ground_angle.transform.rotation = Quaternion.FromToRotation(Vector3.up, ground_ray_hit.normal) * transform.rotation;

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

        move_direction.x = (ground_angle.transform.right.x * current_speed_x) + (ground_angle.transform.forward.x * current_speed_z);
        if(controller.isGrounded) move_direction.y = (ground_angle.transform.right.y * current_speed_x) + (ground_angle.transform.forward.y * current_speed_z);
        move_direction.z = (ground_angle.transform.right.z * current_speed_x) + (ground_angle.transform.forward.z * current_speed_z);

        if (!controller.isGrounded)
        {
            move_direction.y -= 140 * Time.deltaTime;
        }

        //Debug.Log(ground_ray_hit.normal);
        Debug.DrawRay(ground_angle.position, ground_angle.forward, Color.blue, 2);
        Debug.DrawRay(ground_angle.position, ground_angle.right, Color.red, 2);
        Debug.DrawRay(ground_angle.position, ground_angle.up, Color.green, 2);

        controller.Move(move_direction * Time.deltaTime);
	}

    private void LateUpdate()
    {
        look_input += Input.GetAxis("Mouse X") * look_sensitivity;
        transform.rotation = Quaternion.Euler(0, look_input, 0);
    }
}
