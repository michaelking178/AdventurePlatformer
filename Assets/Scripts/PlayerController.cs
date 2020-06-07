using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Entity entity;
    private StateHandler stateHandler;
    private Rigidbody rb;
    private Animator anim;
    private DefaultControls controls;
    private BoxCollider boxCollider;
    private Camera cam;

    private Vector2 movementInput;
    private Ray[] rays = new Ray[9];
    private Vector3 gravityBoost;
    private float doubleJumpDelayModifier = 4f;
    private int jumpCount;
    private float doubleJumpTime = 0f;
    private float rightOffset;
    private float forwardOffset;
    private float currentSpeed;

    [Header("Movement")]
    [SerializeField]
    private float defaultSpeed = 10f;

    [SerializeField]
    private float onTheSpotRotationSpeed = 3f;

    [Header("Jumping")]
    [SerializeField]
    private Vector3 jumpForce;

    [SerializeField]
    private Vector3 doubleJumpForce;

    [Header("CheckForGround Raycasts")]
    [SerializeField]
    float rayLength = 1f;


    private void Awake()
    {
        entity = GetComponent<Entity>();
        controls = new DefaultControls();
        controls.Gameplay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Attack.performed += ctx => StartCoroutine(entity.Attack());
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
        controls.Gameplay.Attack.performed -= ctx => StartCoroutine(entity.Attack());
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        stateHandler = GetComponent<StateHandler>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        gravityBoost = new Vector3(0f, -25f, 0f);
        rightOffset = boxCollider.size.x * 0.5f;
        forwardOffset = boxCollider.size.z * 0.5f;
        currentSpeed = defaultSpeed;
    }

    private void Update()
    {
        CheckForGround();
        DebugStuff();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y >= -9.89f)
        {
            rb.AddForce(gravityBoost);
        }

        if (!stateHandler.Compare("ATTACKING"))
        {
            Move(movementInput);
        }
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

        AlignWithCamera(movementAxis);
    }

    private void AlignWithCamera(Vector2 movementAxis)
    {
        // Rotate the Player upon receiving movement input
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

    private void Jump()
    {
        if (jumpCount == 0)
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            stateHandler.SetState("JUMPING");
            jumpCount++;
            doubleJumpTime = Time.time;
        }
        else if (jumpCount == 1)
        {
            doubleJumpTime = Mathf.Clamp(Time.time - doubleJumpTime, 0f, 0.6f);
            stateHandler.SetState("JUMPING");
            rb.AddForce(doubleJumpForce * (doubleJumpTime * doubleJumpDelayModifier), ForceMode.Impulse);
            anim.SetTrigger("Double Jump");
            jumpCount++;
            doubleJumpTime = 0;
        }
    }

    private void CheckForGround()
    {
        bool isGrounded = false;
        RaycastHit groundHit;
        SetRayPositions();

        for (int i = 0; i < rays.Length; i++)
        {
            rays[i].direction = Vector3.down;
            float rayLength = 1.1f;

            if (Physics.Raycast(rays[i], out groundHit, rayLength) && groundHit.transform.CompareTag("Ground") && !stateHandler.Compare("ATTACKING"))
            {
                isGrounded = true;
                jumpCount = 0;
            }
        }

        if (isGrounded)
        {
            stateHandler.SetState("GROUNDED");
        }
        else
        {
            if (!stateHandler.Compare("JUMPING") && !stateHandler.Compare("ATTACKING"))
            {
                stateHandler.SetState("FALLING");
            }
        }
    }

    private void SetRayPositions()
    {
        rays[0].origin = new Vector3(transform.position.x - rightOffset, transform.position.y + rayLength, transform.position.z + forwardOffset);
        rays[1].origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + forwardOffset);
        rays[2].origin = new Vector3(transform.position.x + rightOffset, transform.position.y + rayLength, transform.position.z + forwardOffset);
        rays[3].origin = new Vector3(transform.position.x - rightOffset, transform.position.y + rayLength, transform.position.z);
        rays[4].origin = new Vector3(transform.position.x, transform.position.y + rayLength, transform.position.z);
        rays[5].origin = new Vector3(transform.position.x + rightOffset, transform.position.y + rayLength, transform.position.z);
        rays[6].origin = new Vector3(transform.position.x - rightOffset, transform.position.y + rayLength, transform.position.z - forwardOffset);
        rays[7].origin = new Vector3(transform.position.x, transform.position.y + rayLength, transform.position.z - forwardOffset);
        rays[8].origin = new Vector3(transform.position.x + rightOffset, transform.position.y + rayLength, transform.position.z - forwardOffset);
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

    private void OnDrawGizmosSelected()
    {
        foreach (Ray ray in rays)
        {
            Gizmos.DrawRay(ray);
            Gizmos.color = Color.blue;
        }
    }
}
