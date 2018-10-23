using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectCamera : MonoBehaviour {

    GameObject game_object;
    Vector3 position;

    const string object_name = "Player";
    const float acceleration = 0.1F;
    const float x_offset = 0F;
    const float y_offset = 1F;
    const float z_offset = -5F;
    
    void Start () {
        game_object = GameObject.Find(object_name);

        position.x = game_object.transform.position.x + x_offset;
        position.y = game_object.transform.position.y + y_offset;
        position.z = game_object.transform.position.z + z_offset;

        transform.position = position;
    }
	
	void Update () {
        position.x = game_object.transform.position.x + x_offset;
        position.y = game_object.transform.position.y + y_offset;
        position.z = game_object.transform.position.z + z_offset;

        transform.position = position;
    }
}
