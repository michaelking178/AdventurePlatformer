using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;

    private float pitch;
    private float yaw;

    // This function is called every fixed framerate frame.
    private void Update()
    {
        pitch += rotationSpeed * Input.GetAxis("CameraYaw");
        yaw += rotationSpeed * Input.GetAxis("Rotate") * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -30f, 30f);
        
        while (yaw < 0f)
        {
            yaw += 360f;
        }
        while (yaw > 360f)
        {
            yaw -= 360f;
        }

        transform.eulerAngles = new Vector3(-pitch, yaw, 0f);
    }
}
