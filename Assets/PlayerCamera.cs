using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    private bool first_person = true;
    private float look_sensitivity = 2.0F;
    private float zoom_sensitivity = 1.0F;
    private float max_look_angle;
    private float min_look_angle;

    private GameObject first_person_object;
    private GameObject third_person_object;
    private Vector3 third_person_object_position;
    private Camera player_camera;
    private Vector3 default_camera_position; // Used for transitioning out of first person mode.
    private Vector3 third_person_position; // Keeps track of camera zoom.
    
    private float horizontal_input, vertical_input;
    private Quaternion camera_quaternion;

    public bool firstPerson
    {
        get { return first_person; }
        set { first_person = value; }
    }

    public float lookSensitivity
    {
        get { return look_sensitivity; }
        set { look_sensitivity = value; }
    }

    public float zoomSensitivity
    {
        get { return zoom_sensitivity; }
        set { zoom_sensitivity = value; }
    }

    void Start()
    {
        first_person_object = GameObject.Find("PlayerController");
        third_person_object = GameObject.Find("PlayerController");
        third_person_object_position = first_person_object.transform.position;
        third_person_position = new Vector3(third_person_object_position.x, third_person_object_position.y, transform.position.z) - third_person_object_position;
    }

    void LateUpdate()
    {
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
            transform.position = first_person_object.transform.position + third_person_position;
            /*
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                first_person = false;
            }
            */
            transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(vertical_input, -75), 75), horizontal_input, 0);
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                first_person = true;
            }

            third_person_object.transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(vertical_input, -75), 75), horizontal_input, 0);

            transform.position = third_person_object.transform.position + (third_person_object.transform.rotation * third_person_position);

            transform.LookAt(third_person_object.transform);
        }
    }
}