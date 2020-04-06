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
    [SerializeField] private float jumpThreshold;

    private float currentSpeed;
    private float sprintSpeed;
    private float previousHeight;
    float cameraXOffset;
    private State state = State.GROUNDED;
    private Rigidbody rb;
    private Camera cam;
    private Animator anim;

    private DefaultControls controls;
    private Vector2 movementInput;

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Jump.performed += ctx => Jump();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Move.performed -= ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Jump.performed -= ctx => Jump();
        controls.Disable();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        anim = GetComponent<Animator>();
        cam = FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        currentSpeed = defaultSpeed;
        sprintSpeed = defaultSpeed * sprintModifier;

        cameraXOffset = cam.transform.position.x;
    }

    void Update()
    {
        DebugStuff();
        CheckState();
    }

    void FixedUpdate()
    {
        Move(movementInput);

        if (state == State.JUMPING && ReachedJumpPeak())
        {
            rb.AddForce(gravityForce);
        }
    }

    private void Move(Vector2 movementAxis)
    {
        Debug.Log(movementAxis);

        // Slow the running animation so that when sprinting, the character looks like they are running faster.
        float animMovementAxis = movementAxis.y;
        float animStrafeAxis = movementAxis.x;

        if (Input.GetButton("Sprint"))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = defaultSpeed;
            animMovementAxis *= 0.95f;
            animStrafeAxis *= 0.95f;
        }

        anim.SetFloat("Speed", animMovementAxis);
        anim.SetFloat("Strafe", animStrafeAxis);

        Vector3 movement = new Vector3(movementAxis.x * Time.fixedDeltaTime * currentSpeed, 0f, movementAxis.y * Time.fixedDeltaTime * currentSpeed);
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

    private void Jump()
    {
        if (state == State.GROUNDED)
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
        }
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

    private void CheckState()
    {
        RaycastHit hit;
        Ray ray = new Ray
        {
            origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z),
            direction = Vector3.down
        };

        if (Physics.Raycast(ray, out hit, 1.05f) && hit.transform.CompareTag("Ground"))
        {
            state = State.GROUNDED;
        }
        else
        {
            state = State.JUMPING;
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
