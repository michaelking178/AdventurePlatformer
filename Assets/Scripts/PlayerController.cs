using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private enum State
    {
        GROUNDED,
        JUMPING,
        FALLING,
        ATTACKING
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
    RaycastHit groundHit;
    Ray ray;

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();

        controls.Gameplay.Sprint.performed += ctx => isSprinting = ctx.ReadValueAsButton();
        controls.Gameplay.Sprint.canceled += ctx => isSprinting = ctx.ReadValueAsButton();

        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Attack.performed += ctx => Attack();
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
        CheckForGround();
        StateManager();
    }

    private void FixedUpdate()
    {
        if (state != State.ATTACKING)
        {
            Move(movementInput);
        }
        rb.AddForce(gravityForce);
    }

    private void StateManager()
    {
        switch (state)
        {
            case State.GROUNDED:
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                SetRunningSpeed(movementInput);
                break;

            case State.JUMPING:
                anim.SetBool("isGrounded", false);
                anim.SetBool("isFalling", false);
                if (ReachedJumpPeak())
                {
                    state = State.FALLING;
                }
                break;

            case State.FALLING:
                anim.SetBool("isFalling", true);
                anim.SetBool("isGrounded", false);
                break;

            case State.ATTACKING:
                break;

            default:
                break;
        }
    }

    private bool ReachedJumpPeak()
    {
        float currentHeight = transform.position.y;
        if (currentHeight - previousHeight < 0.1f)
        {
            return true;
        }
        previousHeight = currentHeight;
        return false;
    }

    private void Move(Vector2 movementAxis)
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

    private void SetRunningSpeed(Vector3 movementAxis)
    {
        // Slow the running animation so that when sprinting, the character looks like they are running faster.
        float animMovementAxis = movementAxis.y;
        float animStrafeAxis = movementAxis.x;

        if (state == State.GROUNDED && isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = defaultSpeed;
            animMovementAxis *= 0.75f;
            animStrafeAxis *= 0.75f;
        }

        anim.SetFloat("Speed", animMovementAxis);
        anim.SetFloat("Strafe", animStrafeAxis);
    }

    private void Jump()
    {
        if (state == State.GROUNDED)
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
            state = State.JUMPING;
            anim.SetTrigger("Jump");
        }
    }

    private void Attack()
    {
        if (state == State.GROUNDED && state != State.ATTACKING)
        {
            anim.SetTrigger("Attack");
            state = State.ATTACKING;
        }
    }

    private void CheckForGround()
    {
        ray.origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        ray.direction = Vector3.down;

        if (Physics.SphereCast(ray, 0.25f, out groundHit, 1f) && groundHit.transform.CompareTag("Ground"))
        {
            state = State.GROUNDED;
        }
        else if (state != State.JUMPING)
        {
            state = State.FALLING;
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
