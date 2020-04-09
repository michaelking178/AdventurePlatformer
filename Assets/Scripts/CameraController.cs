using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private float lerpDelta = 0.1f;

    private float pitch;
    private float yaw;

    private DefaultControls controls;
    private Vector2 cameraInput;

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Look.canceled += ctx => cameraInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + targetOffset, lerpDelta);
        Rotate(cameraInput);
    }

    private void Rotate(Vector2 rotation)
    {
        pitch += rotation.y * rotationSpeed;
        yaw += rotation.x * rotationSpeed;

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
