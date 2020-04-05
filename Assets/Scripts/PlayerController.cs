using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private State state = State.GROUNDED;
    private Rigidbody rb;
    private Camera cam;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        anim = GetComponent<Animator>();
        cam = FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        currentSpeed = defaultSpeed;
        sprintSpeed = defaultSpeed * sprintModifier;
    }

    void Update()
    {
        DebugStuff();
        HandleInput();
        CheckForGround();

        Debug.Log(state);
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        // These two will be used to slow down the running animation so that when sprinting, the character looks like they are running faster.
        float moveAxis = Input.GetAxis("Move");
        float strafeAxis = Input.GetAxis("Strafe");

        if (Input.GetButton("Sprint"))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = defaultSpeed;
            moveAxis *= 0.95f;
            strafeAxis *= 0.95f;
        }

        anim.SetFloat("Speed", moveAxis);
        anim.SetFloat("Strafe", strafeAxis);

        // Control player walking and rotating
        float translation = Input.GetAxis("Move") * Time.fixedDeltaTime * currentSpeed; ;
        float strafe = Input.GetAxis("Strafe") * Time.fixedDeltaTime * currentSpeed;

        // Rotate the character upon receiving movement input
        if (Input.GetAxis("Move") != 0 || Input.GetAxis("Strafe") != 0)
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

        // Apply movement
        transform.Translate(strafe, 0, translation);
    }

    private void HandleInput()
    {
        switch (state)
        {
            case State.GROUNDED:
                if (Input.GetButtonDown("Jump"))
                {
                    rb.AddForce(jumpForce, ForceMode.Impulse);
                    anim.SetTrigger("Jump");
                }
                break;
            case State.JUMPING:
                if (ReachedJumpPeak())
                {
                    rb.AddForce(gravityForce);
                }
                break;
            default:
                break;
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

    private void CheckForGround()
    {
        RaycastHit hit;
        Ray ray = new Ray
        {
            origin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z),
            direction = Vector3.down
        };

        if (Physics.Raycast(ray, out hit, 1.05f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                state = State.GROUNDED;
            }
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
