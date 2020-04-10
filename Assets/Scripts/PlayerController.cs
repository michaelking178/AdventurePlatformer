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

    [Header("Jump")]
    [SerializeField] private Vector3 jumpForce;
    [SerializeField] private Vector3 doubleJumpForce;
    [SerializeField] private float doubleJumpDelayModifier = 4f;
    [SerializeField] private Vector3 gravityForce;

    [Header("Rotation")]
    [SerializeField] private float onTheSpotRotationSpeed = 3f;

    private float currentSpeed;
    private float sprintSpeed;
    private float previousHeight;
    private State state;
    private Rigidbody rb;
    private Camera cam;
    private Animator anim;
    private int jumpCount;

    private DefaultControls controls;
    private Vector2 movementInput;
    private Ray ray;
    private State previousState;

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Attack.performed += ctx => StartCoroutine(Attack());
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
        controls.Gameplay.Attack.performed -= ctx => StartCoroutine(Attack());
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        anim = GetComponent<Animator>();
        cam = FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        currentSpeed = defaultSpeed;
    }

    private void Update()
    {
        DebugStuff();
        StateManager();
        CheckForGround();
    }

    private void FixedUpdate()
    {
        rb.AddForce(gravityForce);

        if (state != State.ATTACKING)
        {
            Move(movementInput);
        }
    }

    private void StateManager()
    {
        switch (state)
        {
            case State.GROUNDED:
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                jumpCount = 0;
                break;

            case State.JUMPING:
                anim.SetBool("isGrounded", false);
                anim.SetBool("isFalling", false);
                if (ReachedJumpPeak())
                {
                    SetState(State.FALLING);
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
        if (currentHeight - previousHeight < -0f)
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


    float doubleJumpTime = 0f;

    private void Jump()
    {
        if (jumpCount == 0)
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            SetState(State.JUMPING);
            jumpCount++;
            doubleJumpTime = Time.time;
        }
        else if (jumpCount == 1)
        {
            doubleJumpTime = Time.time - doubleJumpTime;
            Debug.Log("Double Jump Time: " + doubleJumpTime);
            SetState(State.JUMPING);
            rb.AddForce(doubleJumpForce * (doubleJumpTime * doubleJumpDelayModifier), ForceMode.Impulse);
            anim.SetTrigger("Double Jump");
            jumpCount++;
            doubleJumpTime = 0;
        }
    }

    private IEnumerator Attack()
    {
        if (state == State.GROUNDED && state != State.ATTACKING) // Must be grounded, and cannot attack while already attacking!
        {            
            anim.SetTrigger("Attack");
            SetState(State.ATTACKING);
            yield return new WaitForSeconds(0.5f);
            SetState(previousState);
        }
    }

    private void CheckForGround()
    {
        RaycastHit groundHit;
        ray.origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        ray.direction = Vector3.down;

        if (Physics.Raycast(ray, out groundHit, 1.01f) && groundHit.transform.CompareTag("Ground") && state != State.ATTACKING)
        {
            SetState(State.GROUNDED);
        }
        else if (state != State.JUMPING && state != State.ATTACKING)
        {
            SetState(State.FALLING);
        }
    }

    private void SetState(State newState)
    {
        if (previousState != newState)
        {
            previousState = state;
        }
        state = newState;
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
