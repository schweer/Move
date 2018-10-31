using UnityEngine;
using System.Collections;

//test

public class PlayerController : MonoBehaviour
{
    public float current_speed_x = 0.0f;
    public float current_speed_z = 0.0f;
    public float ground_acceleration = 100.0f;
    public float aerial_acceleration = 100.0f;
    public float acceleration = 100.0f;
    public float max_speed = 20.0f;
    public float jump_speed = 10.0f;
    public float gravity = 30.0f;
    public float dash_distance = 20.0f;
    public Vector3 drag = new Vector3(0.2f, 0.2f, 0.2f);
    public float look_sensitivity = 2.0F;

    private Vector3 move_direction = Vector3.zero;
    private CharacterController controller;
    private float horizontal_input, vertical_input, look_input;
    private LayerMask ground_layer;
    private Ray ground_ray;
    private RaycastHit ground_ray_hit;
    private bool grounded;
    private float airtime;
    private float angle;
    
    private KeyCode dash_key = KeyCode.Mouse1;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        ground_layer = LayerMask.NameToLayer("Ground");
        ground_ray.direction = Vector3.down;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        RunDebuggers();

        ground_ray.origin = transform.position;
            
        grounded = Physics.Raycast(ground_ray, out ground_ray_hit, controller.bounds.extents.y * 1.1f); // ground check
        Physics.Raycast(ground_ray, out ground_ray_hit, Mathf.Infinity); // ground angle check

        angle = Vector3.Angle(ground_ray_hit.normal, -transform.up) - 180;

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (grounded)
        {
            acceleration = ground_acceleration;
        }
        else
        {
            acceleration = aerial_acceleration;
        }

        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");

        /*
        move_direction.x = (transform.right.x * horizontal_input * acceleration) + (transform.forward.x * vertical_input * acceleration);
        move_direction.z = (transform.right.z * horizontal_input * acceleration) + (transform.forward.z * vertical_input * acceleration);
        */

        if (horizontal_input > 0)
        {
            current_speed_x = Mathf.Min(horizontal_input * acceleration, max_speed);
            //move_direction.x = transform.right.x * horizontal_input * acceleration;
        }
        if (vertical_input > 0)
        {
            current_speed_z = Mathf.Min(vertical_input * acceleration, max_speed);
            //move_direction.z = transform.forward.z * vertical_input * acceleration;
        }
        if (horizontal_input < 0)
        {
            current_speed_x = Mathf.Max(horizontal_input * acceleration, -max_speed);
            //move_direction.x = transform.right.x * horizontal_input * acceleration;
        }
        if (vertical_input < 0)
        {
            current_speed_z = Mathf.Max(vertical_input * acceleration, -max_speed);
            //move_direction.z = transform.forward.z * vertical_input * acceleration;
        }
        if (horizontal_input == 0)
        {
            current_speed_x = horizontal_input;
            //move_direction.x = transform.right.x * horizontal_input * acceleration;
        }
        if (vertical_input == 0)
        {
            current_speed_z = vertical_input;
            //move_direction.z = transform.forward.z * vertical_input * acceleration;
        }

        move_direction.x = (transform.right.x * current_speed_x) + (transform.forward.x * current_speed_z);
        move_direction.z = (transform.right.z * current_speed_x) + (transform.forward.z * current_speed_z);

        /*
        move_direction.x = current_speed_x;
        move_direction.z = current_speed_z;
        */

        /*
        if (Input.GetKey(dash_key) && move_direction.x > 0)
        {
            move_direction.x += dash_distance;
        }
        if (Input.GetKey(dash_key) && move_direction.x < 0)
        {
            move_direction.x -= dash_distance;
        }
        if (Input.GetKey(dash_key) && move_direction.z > 0)
        {
            move_direction.z += dash_distance;
        }
        if (Input.GetKey(dash_key) && move_direction.z < 0)
        {
            move_direction.z -= dash_distance;
        }
        */

        if (grounded && move_direction.y < 0)
        {
            move_direction.y = 0f; // Keeps alternating between grounded and not grounded
        }

        if ((grounded || controller.isGrounded) && Input.GetButton("Jump"))
        {
            move_direction.y = jump_speed;
        }

        if (!grounded || controller.isGrounded)
        {
            move_direction.y -= (gravity * Time.deltaTime);
        }
        
        controller.Move(move_direction * Time.deltaTime);

        /*
        move_direction.x /= 1 + drag.x * Time.deltaTime;
        move_direction.y /= 1 + drag.y * Time.deltaTime;
        move_direction.z /= 1 + drag.z * Time.deltaTime;
        */
    }

    private void LateUpdate()
    {
        look_input += Input.GetAxis("Mouse X") * look_sensitivity;
        transform.rotation = Quaternion.Euler(0, look_input, 0);
    }

    private void RunDebuggers()
    {
        //Debug.Log("move_direction.x, move_direction.z: " + move_direction.x + ", " + move_direction.z);
        Debug.Log("grounded: " + grounded + " isGrounded: " + controller.isGrounded + " angle: " + angle + " magnitude: " + move_direction.magnitude);

        // draw ground_ray
        //Debug.DrawRay(transform.position, Vector3.down * controller.bounds.extents.y * 1.1f, Color.red, 1.0f);
        
        /*
        // airtime
        if (!grounded)
        {
            airtime++;
        }
        if (grounded)
        {
            if (airtime > 0)
            {
                Debug.Log("airtime: " + airtime);
                airtime = 0;
            }
        }
        */
    }
}