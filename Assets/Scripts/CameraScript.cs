using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public float sensitivity = 150.0f;
    public float pitch, yaw = 0.0f;
    bool ready = false;

	// Use this for initialization
	void Start () {
        pitch = transform.localRotation.eulerAngles.x;
        yaw = transform.localRotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update () {
        pitch += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
	}
}
