using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public bool first_person = true;
    public float look_sensitivity = 1.0F;
    public float zoom_sensitivity = 1.0F;

    private GameObject player;
    private Vector3 default_camera_position; // Used for transitioning out of first person mode.
    private Vector3 third_person_position; // Keeps track of camera zoom.
    private Vector3 heading;
    
    private float horizontal_input, vertical_input;
    private Quaternion cameraQuaternion;

    void Start()
    {
        player = GameObject.Find("Player");
        third_person_position = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + new Vector3(0, 2, 0);

        horizontal_input += Input.GetAxis("Mouse X") * look_sensitivity;
        vertical_input -= Input.GetAxis("Mouse Y") * look_sensitivity;
        if (vertical_input > 0)
        {
            vertical_input = Mathf.Min(75, vertical_input);
        }
        else
        {
            vertical_input = Mathf.Max(-75, vertical_input);
        }

        if (first_person)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                first_person = false;
            }

            transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(vertical_input, -75), 75), horizontal_input, 0);
        }
        else
        {
            /*
            // lerp for smooth movement
            heading = player.transform.position - transform.position;
            float axis = Input.GetAxis("Mouse ScrollWheel");
            if (axis != 0)
            {
                if (heading.magnitude < 2)
                {
                    first_person = true;
                }
                else
                {
                    third_person_position += heading * axis * zoom_sensitivity;
                }
            }
            */

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                first_person = true;
            }

            player.transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(vertical_input, -75), 75), horizontal_input, 0);

            transform.position = player.transform.position + (player.transform.rotation * third_person_position);

            transform.LookAt(player.transform);
        }
    }
}