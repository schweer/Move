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
    
    private float xRotate, yRotate;
    private Quaternion cameraQuaternion;

    void Start()
    {
        player = GameObject.Find("Player");
        default_camera_position = transform.position;
        third_person_position = default_camera_position;
    }

    void LateUpdate()
    {
        if (first_person)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.position = default_camera_position;
                first_person = false;
            }

            transform.position = player.transform.position + new Vector3(0, 2, 0);

            cameraQuaternion = Quaternion.Euler(0, xRotate, 0);
            GameObject.Find("Player").transform.rotation = cameraQuaternion;
            xRotate += Input.GetAxis("Mouse X") * look_sensitivity;
            yRotate -= Input.GetAxis("Mouse Y") * look_sensitivity;
            if (yRotate > 0)
            {
                yRotate = Mathf.Min(75, yRotate);
            }
            else
            {
                yRotate = Mathf.Max(-75, yRotate);
            }
            transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(yRotate, -75), 75), xRotate, 0);
        }
        else
        {
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

            transform.position = player.transform.position + third_person_position;

            cameraQuaternion = Quaternion.Euler(0, xRotate, 0);
            GameObject.Find("Player").transform.rotation = cameraQuaternion;
            xRotate += Input.GetAxis("Mouse X") * look_sensitivity;
            yRotate -= Input.GetAxis("Mouse Y") * look_sensitivity;
            if (yRotate > 0)
            {
                yRotate = Mathf.Min(75, yRotate);
            }
            else
            {
                yRotate = Mathf.Max(-75, yRotate);
            }
            transform.rotation = Quaternion.Euler(Mathf.Min(Mathf.Max(yRotate, -75), 75), xRotate, 0);
        }
    }
}