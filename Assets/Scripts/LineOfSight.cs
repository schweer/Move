using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour {

    private RaycastHit vision;
    public float rayLength;
    private bool isGrabbed;
    private Rigidbody grabbedObject;

	void Start ()
    {
        rayLength = 4.0f;
        isGrabbed = false;
	}
	
	void Update ()
    {
        Debug.DrawRay(transform.position, transform.forward * rayLength, Color.red, 0.5f);

        if (Physics.Raycast(transform.position, transform.forward, out vision, rayLength))
        {
            if (vision.collider.tag == "Interactive")
            {
                Debug.Log(vision.collider.name);
                if (Input.GetKeyDown(KeyCode.E) && !isGrabbed)
                {
                    grabbedObject = vision.rigidbody;
                    grabbedObject.isKinematic = true;
                    grabbedObject.transform.SetParent(gameObject.transform);
                    isGrabbed = true;
                }
                else if (isGrabbed && Input.GetKeyDown(KeyCode.E))
                {
                    grabbedObject.transform.parent = null;
                    grabbedObject.isKinematic = false;
                    isGrabbed = false;
                }
            }
        }
	}
}
