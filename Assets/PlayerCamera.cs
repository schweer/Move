using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public GameObject player;
    public Camera player_camera;

    public Vector3 offset;

    public float rotation_speed = 3.0F;

    void Start()
    {
        player = GameObject.Find("Player");
        player_camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
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

        Quaternion neededRotation = Quaternion.LookRotation(point - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, neededRotation, Time.deltaTime * rotation_speed);
    }
}