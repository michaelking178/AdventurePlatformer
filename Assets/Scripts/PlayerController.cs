using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField]
    private Vector3 jumpForce;

    [SerializeField]
    private Vector3 doubleJumpForce;

    [SerializeField]
    private float doubleJumpDelayModifier = 4f;

    [SerializeField]
    private Vector3 gravityBoost;

    private float doubleJumpTime = 0f;
    private Rigidbody rb;
    private Animator anim;
    private Movement movement;
    private StateHandler stateHandler;
    private int jumpCount;

    private DefaultControls controls;
    private Vector2 movementInput;
    private Ray[] rays = new Ray[9];
    private float rightOffset = 0.25f;
    private float forwardOffset = 0.2f;

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
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        stateHandler = GetComponent<StateHandler>();
    }

    private void Update()
    {
        DebugStuff();
        CheckForGround();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y >= -9.89f)
        {
            rb.AddForce(gravityBoost);
        }

        if (!stateHandler.Compare("ATTACKING"))
        {
            movement.Move(movementInput);
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

    private IEnumerator Attack()
    {
        if (!stateHandler.Compare("ATTACKING"))
        {
            if (stateHandler.Compare("GROUNDED")) // Must be grounded, and cannot attack while already attacking!
            {
                anim.SetTrigger("Attack");
                stateHandler.SetState("ATTACKING");
                yield return new WaitForSeconds(0.5f);
                stateHandler.Revert();
            }
            else if (stateHandler.Compare("JUMPING") || stateHandler.Compare("FALLING"))
            {
                anim.SetTrigger("Jump Attack");
                stateHandler.SetState("ATTACKING");
                yield return new WaitForSeconds(0.5f);
                stateHandler.Revert();
            }
        }
    }

    private void CheckForGround()
    {
        bool isGrounded = false;
        RaycastHit groundHit;
        SetRayPositions();

        for(int i = 0; i < rays.Length; i++)
        {
            rays[i].direction = Vector3.down;

            if (Physics.Raycast(rays[i], out groundHit, 1.01f) && groundHit.transform.CompareTag("Ground") && !stateHandler.Compare("ATTACKING"))
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
        rays[0].origin = new Vector3(transform.position.x - rightOffset, transform.position.y + 1f, transform.position.z + forwardOffset);
        rays[1].origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + forwardOffset);
        rays[2].origin = new Vector3(transform.position.x + rightOffset, transform.position.y + 1f, transform.position.z + forwardOffset);
        rays[3].origin = new Vector3(transform.position.x - rightOffset, transform.position.y + 1f, transform.position.z);
        rays[4].origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        rays[5].origin = new Vector3(transform.position.x + rightOffset, transform.position.y + 1f, transform.position.z);
        rays[6].origin = new Vector3(transform.position.x - rightOffset, transform.position.y + 1f, transform.position.z - forwardOffset);
        rays[7].origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - forwardOffset);
        rays[8].origin = new Vector3(transform.position.x + rightOffset, transform.position.y + 1f, transform.position.z - forwardOffset);
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
