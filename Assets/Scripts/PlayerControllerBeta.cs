using System;
using UnityEngine;

public class PlayerControllerBeta : MonoBehaviour
{
    // Attributes
    private float speed;
    private float acceleration = 100f;
    private float max_run_speed = 12.0f;
    private float gravity = 45.0f;
    private float slope_force = 10.0f;
    private float jump_height = 2.0f;
    private float look_sensitivity = 2.0f;
    private float current_speed_x, current_speed_z;
    private DateTimeOffset lastDash = DateTime.UtcNow;
    private double dashCooldown = .5 * 10000000; //first number is in SECONDS, operator converts to ticks

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
    private Vector3 direction;
    private Vector3 flat_direction;
    private Ray ground_ray;
    private Ray slope_ray;
    private RaycastHit ground_ray_hit;
    private RaycastHit slope_ray_hit;
    private ControllerColliderHit wall_hit;
    
    // Components
    private CharacterController controller;
    private Transform slope_transform;

    private bool on_wall = false;

	private void Start ()
    {
        controller = GetComponent<CharacterController>();
        speed = max_run_speed;
        slope_transform = transform.GetChild(0);
        ground_ray = new Ray(transform.position, Vector3.down);
        slope_ray = new Ray(slope_transform.position, -slope_transform.up);
        Cursor.lockState = CursorLockMode.Locked;
        //Debug.Log("ground_angle: " + ground_angle + " controller: " + controller);
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (on_wall == true && speed > max_run_speed && (controller.collisionFlags == CollisionFlags.Sides))
        {
            on_wall = true;
        }
        else
        {
            on_wall = false;
        }

        direction = transform.position - ground_ray.origin;
        flat_direction = new Vector3(direction.x, 0, direction.z);

        CalculateGroundRay();
        
        slope_transform.rotation = Quaternion.FromToRotation(Vector3.up, ground_ray_hit.normal) * transform.rotation; // Rotate slope ray based on angle beneath player

        CalculateSlopeRay();
        
        angle = Vector3.SignedAngle(direction, flat_direction, Vector3.Cross(Vector3.up, flat_direction));
        //Debug.Log("angle: " + angle);
        
        UpdateSpeed();
        Debug.Log("speed:" + speed + " increase: " + (speed - speed * 0.99f) + " direction: " + direction + " angle: " + angle);
        
        current_speed_x = HorizontalInput();
        current_speed_z = VerticalInput();
        //Debug.Log("current_speed_x: " + current_speed_x + " current_speed_z: " + current_speed_z);
        if ((OnSlope() || controller.isGrounded) && !on_wall)
        {
            move_direction.x = (slope_transform.right.x * current_speed_x) + (slope_transform.forward.x * current_speed_z);
            if(controller.isGrounded)move_direction.y = (slope_transform.right.y * current_speed_x) + (slope_transform.forward.y * current_speed_z);
            move_direction.z = (slope_transform.right.z * current_speed_x) + (slope_transform.forward.z * current_speed_z);
        }
        if(!OnSlope() && !controller.isGrounded && !on_wall)
        {
            move_direction.x = (transform.right.x * current_speed_x) + (transform.forward.x * current_speed_z);
            move_direction.z = (transform.right.z * current_speed_x) + (transform.forward.z * current_speed_z);
        }

        if (!controller.isGrounded && !OnSlope() && Input.GetKeyDown(KeyCode.CapsLock))
        {
            FastFall();
        }

        if (controller.isGrounded)
        {
            move_direction.y = -5f; // Keeps character from poping off of peaks
        }

        if (OnSlope()) // if(OnSlope() && !Dashing())
        {
            move_direction.y = -slope_force * speed; // Holds character to slopes at high speed
        }

        if ((OnSlope() || controller.isGrounded || on_wall) && Input.GetKey(KeyCode.Space))
        {
            Jump();
        }

        if ((OnSlope() || controller.isGrounded) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (ChainDashOffCooldown())
            {
                Chaindash();
            }
        }

        move_direction.y -= Gravity();
        Decelerate();
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
        if ((!controller.isGrounded || OnSlope()) && move_direction.y > -gravity)
        {
            return gravity * Time.deltaTime;
        }
        else
        {
            return 0.0f;
        }
    }

    private void UpdateSpeed()
    {
        if (current_speed_x == 0 && current_speed_z == 0)
        {
            speed = max_run_speed;
        }

        if (IsDashing())
        { 

        }

        if (IsSliding() && OnSlope())
        {
            speed *= 1.0f - angle * 0.03f * Time.deltaTime;
        }

        if ((OnSlope() || controller.isGrounded) && Input.GetKey(KeyCode.Space))
        {

        }
    }

    private void Decelerate()
    {
        if (speed > max_run_speed)
        {
            speed *= 0.99f;
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Vector3.Angle(hit.normal, Vector3.up) > 89.9f && Vector3.Angle(hit.normal, Vector3.up) < 90.1f && hit.point.y > transform.position.y)
        {
            on_wall = true;
            wall_hit = hit;
            return;
        }
    }

    #endregion

    #region Game States

    private bool IsRunning()
    {
        if (controller.isGrounded && !IsSliding() && !IsDashing())
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

    private bool IsDashing()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            return true;
        }

        else return false;
    }

    private bool ChainDashOffCooldown()
    {
        double debugElapsed = (DateTimeOffset.UtcNow.UtcTicks - lastDash.UtcTicks) / 10000000;
        //Debug.Log(debugElapsed);

        if ((DateTimeOffset.UtcNow.UtcTicks - lastDash.UtcTicks) >= dashCooldown)
        {
            lastDash = DateTimeOffset.UtcNow;
            return true;
        }
        else
        {
            //Debug.Log("failing cooldown check");
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

    private void Jump()
    {
        if (!Input.GetKey(KeyCode.CapsLock) && !on_wall)
        {
            move_direction.y = Mathf.Sqrt(2 * jump_height * gravity);
        }

        if (Input.GetKey(KeyCode.CapsLock) && !on_wall)
        {
            move_direction = new Vector3(move_direction.x, slope_transform.up.y * Mathf.Sqrt(2 * jump_height * gravity), move_direction.z);
        }

        if (on_wall)
        {
            move_direction = (Vector3.up + wall_hit.normal) * speed;
            on_wall = false;
        }
    }

    #endregion

    #region Advanced Movement
    private void Chaindash()
    {
        speed = speed * 2;
    }

    private void FastFall()
    {
        move_direction.y = -gravity;
    }
    #endregion
}
