using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    void Update()
    {
        // Spin the object around the world origin at 20 degrees/second.
        transform.RotateAround(Vector3.zero, Vector3.up, 180 * Time.deltaTime);
    }
}