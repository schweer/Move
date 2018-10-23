using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

    Vector3 position;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            position = transform.position;
            position.x--;
            transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            position = transform.position;
            position.x++;
            transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            position = transform.position;
            position.z++;
            transform.position = position;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            position = transform.position;
            position.z--;
            transform.position = position;
        }
    }
}
