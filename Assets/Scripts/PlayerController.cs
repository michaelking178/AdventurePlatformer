using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float defaultSpeed = 10f;
    [SerializeField] private float sprintModifier = 1.75f;

    [Header("Jump")]
    [SerializeField] private Vector3 jumpForce;
    [SerializeField] private Vector3 gravityForce;
    [SerializeField] private float jumpThreshold;

    private float currentSpeed;
    private float diagonalSpeed;

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
        currentSpeed = defaultSpeed;
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

        // Rotate the character upon receiving movement input
        if (Input.GetAxis("Move") != 0 || Input.GetAxis("Strafe") != 0)
        {
            // Direction to rotate toward is the inverse of the player => camera direction
            Vector3 camPos = new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
            Vector3 targetDirection = transform.position - camPos;

            // Determine necessary rotation to face the target direction
            float singleStep = currentSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Apply rotation and movement
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

    void OnTriggerStay(Collider col)
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
