using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public bool first_person = true;
    public float look_sensitivity = 2.0F;
    public int zoom_sensitivity = 1;

    private GameObject player;
    
    private float xRotate, yRotate;
    private Quaternion cameraQuaternion;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void LateUpdate()
    {
        if (first_person)
        {
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
            float axis = Input.GetAxisRaw("Mouse ScrollWheel");
            transform.position += player.transform.forward * axis * zoom_sensitivity;
            Debug.Log(axis * zoom_sensitivity);
        }
    }
}