using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float yaw;

    // This function is called every fixed framerate frame.
    void FixedUpdate()
    {
        yaw = Input.GetAxis("CameraYaw");
        transform.Rotate(new Vector3(-yaw, 0, 0));
    }
}
