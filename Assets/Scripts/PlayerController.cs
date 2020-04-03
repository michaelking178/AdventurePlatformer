﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float currentSpeed;
    [SerializeField] private float sprintModifier;
    [SerializeField] private Vector3 jumpForce;
    [SerializeField] private Vector3 gravityForce;
    [SerializeField] private float jumpThreshold;

    private float diagonalSpeed;
    private float defaultSpeed;
    private bool isGrounded;
    private float previousHeight;
    private Rigidbody rb;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = FindObjectOfType<Camera>();

        rb = GetComponent<Rigidbody>();
        defaultSpeed = currentSpeed;
        diagonalSpeed = currentSpeed * 0.87f;
    }

    void Update()
    {
        DebugStuff();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            StartCoroutine(Jump());
        }
        if(!isGrounded && ReachedJumpPeak())
        {
            rb.AddForce(gravityForce);
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (Input.GetButton("Sprint"))
        {
            currentSpeed *= sprintModifier;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            currentSpeed = defaultSpeed;
        }

        // Control player walking and rotating
        float translation = Input.GetAxis("Move") * currentSpeed;
        float strafe = Input.GetAxis("Strafe") * currentSpeed;

        if (Mathf.Abs(strafe) > 0 && Mathf.Abs(translation) > 0)
        {
            currentSpeed = diagonalSpeed;
        }
        else
        {
            currentSpeed = defaultSpeed;
        }

        translation *= Time.fixedDeltaTime;
        strafe *= Time.fixedDeltaTime;

        if (Input.GetAxis("Move") != 0 || Input.GetAxis("Strafe") != 0)
        {

            // TODO: Gotta fix the x-rotation. It should stay clamped at 0 when moving. Otherwise I get my little guy Naruto running.
            Vector3 targetDirection = transform.position - cam.transform.position;

            float singleStep = currentSpeed * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.Translate(strafe, 0, translation);
        }
    }

    private IEnumerator Jump()
    {
        // Control player jumping
        rb.AddForce(jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        isGrounded = false;
    }

    private bool ReachedJumpPeak()
    {
        float currentHeight = transform.position.y;
        if (currentHeight - previousHeight < jumpThreshold)
        {
            return true;
        }
        previousHeight = currentHeight;
        return false;
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void DebugStuff()
    {
        // Cursor lock ESC key
        if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
