using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public float look_sensitivity = 2.0F;
    public float scroll_sensitivity = 2.0F;

    private GameObject player;
    private Camera player_camera;

    private float scroll_wheel_movement;
    private Vector3 camera_zoom;
    private Vector3 offset;
    
    private float xRotate, yRotate;
    private Quaternion cameraQuaternion;

    void Start()
    {
        player = GameObject.Find("Player");
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        scroll_wheel_movement = Input.GetAxis("Mouse ScrollWheel") * scroll_sensitivity;

        transform.position = player.transform.position + offset;

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

    void OnGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = player_camera.pixelHeight - currentEvent.mousePosition.y;

        point = player_camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, player_camera.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + player_camera.pixelWidth + ":" + player_camera.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
    }
}