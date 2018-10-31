using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    private float look_sensitivity = 2.0F;
    private float max_look_angle = 75;
    private float min_look_angle = -75;
    private GameObject look_object;
    private Vector3 offset;
    private float horizontal_input, vertical_input;
    private Quaternion camera_quaternion;

    void Start()
    {
        look_object = GameObject.Find("PlayerController");
        //offset = transform.position - look_object.transform.position;
        offset = new Vector3(0, 1, 0);
    }

    void LateUpdate()
    {
        horizontal_input += Input.GetAxis("Mouse X") * look_sensitivity;
        vertical_input -= Input.GetAxis("Mouse Y") * look_sensitivity;

        if (vertical_input > 0)
        {
            vertical_input = Mathf.Min(max_look_angle, vertical_input);
        }
        else
        {
            vertical_input = Mathf.Max(min_look_angle, vertical_input);
        }

        transform.position = look_object.transform.position + offset;

        transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(vertical_input, -75), 75), horizontal_input, 0);
    }
}