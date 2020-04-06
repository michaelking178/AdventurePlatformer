using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private enum State
    {
        GROUNDED,
        JUMPING
    };

    [Header("Speed")]
    [SerializeField] private float defaultSpeed = 10f;
    [SerializeField] private float sprintModifier = 1.75f;

    [Header("Jump")]
    [SerializeField] private Vector3 jumpForce;
    [SerializeField] private Vector3 gravityForce;

    private float currentSpeed;
    private float sprintSpeed;
    private float previousHeight;
    private State state;
    private Rigidbody rb;
    private Camera cam;
    private Animator anim;
    private bool isSprinting;

    private DefaultControls controls;
    private Vector2 movementInput;
    RaycastHit hit;
    Ray ray;

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Sprint.performed += ctx => isSprinting = ctx.ReadValueAsButton();
        controls.Gameplay.Sprint.canceled += ctx => isSprinting = ctx.ReadValueAsButton();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Move.performed -= ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled -= ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Jump.performed -= ctx => Jump();
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        anim = GetComponent<Animator>();
        cam = FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        currentSpeed = defaultSpeed;
        sprintSpeed = defaultSpeed * sprintModifier;
    }

    private void Update()
    {
        DebugStuff();
        CheckState();
    }

    private void FixedUpdate()
    {
        Move(movementInput);
        
        if (state == State.JUMPING)
        {
            rb.AddForce(gravityForce);
        }
    }

    private void Move(Vector2 movementAxis)
    {
        CheckForSprint(movementAxis);

        Vector3 movement = new Vector3(movementAxis.x * Time.fixedDeltaTime * currentSpeed, 0f, movementAxis.y * Time.fixedDeltaTime * currentSpeed);
        if (movement.z < 0f)
        {
            movement.z *= 0.5f;
        }
        transform.Translate(movement);

        // Rotate the character upon receiving movement input
        if (movementAxis.x != 0 || movementAxis.y != 0)
        {
            // Direction to rotate toward
            Vector3 camPos = new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
            Vector3 targetDirection = transform.position - camPos;

            // Determine rotation needed to face the target direction
            float singleStep = currentSpeed * Time.fixedDeltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Apply rotation
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void CheckForSprint(Vector3 movementAxis)
    {
        // Slow the running animation so that when sprinting, the character looks like they are running faster.
        float animMovementAxis = movementAxis.y;
        float animStrafeAxis = movementAxis.x;

        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = defaultSpeed;
            animMovementAxis *= 0.85f;
            animStrafeAxis *= 0.85f;
        }

        anim.SetFloat("Speed", animMovementAxis);
        anim.SetFloat("Strafe", animStrafeAxis);
    }

    private void Jump()
    {
        if (state == State.GROUNDED)
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            state = State.JUMPING;
        }
    }

    private void CheckState()
    {
        ray.origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        ray.direction = Vector3.down;

        if (Physics.Raycast(ray, out hit, 1.01f) && hit.transform.CompareTag("Ground"))
        {
            state = State.GROUNDED;
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
