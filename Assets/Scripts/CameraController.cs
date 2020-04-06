using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //private float pitch;
    //private float yaw;

    //// This function is called every fixed framerate frame.
    //private void FixedUpdate()
    //{
    //    pitch += Input.GetAxis("CameraYaw");
    //    yaw += Input.GetAxis("Rotate") * Time.deltaTime;

    //    pitch = Mathf.Clamp(pitch, -30f, 30f);

    //    while (yaw < 0f)
    //    {
    //        yaw += 360f;
    //    }
    //    while (yaw > 360f)
    //    {
    //        yaw -= 360f;
    //    }

    //    transform.eulerAngles = new Vector3(-pitch, yaw, 0f);
    //}

    private float pitch;
    private float yaw;

    private DefaultControls controls;

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Look.performed += ctx => Rotate(ctx.ReadValue<Vector2>());
        controls.Gameplay.Look.canceled += ctx => Rotate(ctx.ReadValue<Vector2>());

    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Rotate(Vector2 rotation)
    {
        pitch += rotation.y;
        yaw += rotation.x;

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
