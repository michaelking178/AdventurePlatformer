using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float defaultSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float onTheSpotRotationSpeed = 3f;

    private Animator anim;
    private Camera cam;

    private float currentSpeed;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentSpeed = defaultSpeed;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void Move(Vector2 movementAxis)
    {
        Vector3 movement = new Vector3(movementAxis.x * Time.fixedDeltaTime * currentSpeed, 0f, movementAxis.y * Time.fixedDeltaTime * currentSpeed);

        // Slow down strafe movement a bit
        movement.x *= 0.85f;

        // Slow down backward movement
        if (movement.z < 0f)
        {
            movement.z *= 0.5f;
        }

        transform.Translate(movement);
        anim.SetFloat("Speed", movementAxis.y);
        anim.SetFloat("Strafe", movementAxis.x);

        // Rotate the character upon receiving movement input
        if (movementAxis.x != 0 || movementAxis.y != 0)
        {
            // Direction to rotate toward
            Vector3 camPos = new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
            Vector3 targetDirection = transform.position - camPos;

            // Determine rotation needed to face the target direction
            float singleStep = currentSpeed * onTheSpotRotationSpeed * Time.fixedDeltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Apply rotation
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
