using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private float lerpDelta = 0.1f;

    private float pitch;
    private float yaw;

    private DefaultControls controls;
    private Vector2 cameraInput;

    private Camera cam;
    private PlayerController player;
    private Vector3 camLocalPos;
    private float defaultDistanceToCam;
    private float distanceToCam;
    private LayerMask mask;
    private RaycastHit hit;

    [SerializeField] private float zoomLerpDelta = 0.1f;
    [SerializeField] private Vector3 cameraZoomOffset = new Vector3();

    private void Awake()
    {
        controls = new DefaultControls();
        controls.Gameplay.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Look.canceled += ctx => cameraInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        camLocalPos = cam.transform.localPosition;
        defaultDistanceToCam = Vector3.Distance(player.transform.position, cam.transform.position);
        mask = LayerMask.GetMask("NotClippable");
    }

    void Update()
    {
        Vector3 direction = cam.transform.position - player.transform.position;
        distanceToCam = Vector3.Distance(player.transform.position, cam.transform.position);
        float rayOffset = 1f;

        if (Physics.Raycast(player.transform.position, direction, out hit, distanceToCam + rayOffset, mask))
        {
            float distanceToCollision = Vector3.Distance(player.transform.position, hit.point);

            if (distanceToCam >= distanceToCollision)
            {
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, player.transform.position + cameraZoomOffset, zoomLerpDelta);
                distanceToCam = Vector3.Distance(player.transform.position, cam.transform.position);
            }
        }
        else
        {
            ResetCamera(direction);
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + targetOffset, lerpDelta);
        Rotate(cameraInput);
    }

    private void Rotate(Vector2 rotation)
    {
        pitch += rotation.y * rotationSpeed;
        yaw += rotation.x * rotationSpeed;

        pitch = Mathf.Clamp(pitch, -30f, 30f);

        while (yaw < 0f)
        {
            yaw += 360f;
        }
        while (yaw > 360f)
        {
            yaw -= 360f;
        }

        transform.eulerAngles = new Vector3(-pitch, yaw, 0f);
    }

    private void ResetCamera(Vector3 direction)
    {
        if (distanceToCam < defaultDistanceToCam && CamIsOutsideDeadzone())
        {
            Vector3 newCamPos = Vector3.Lerp(cam.transform.localPosition, camLocalPos, zoomLerpDelta);
            cam.transform.localPosition = newCamPos;
        }
    }

    private bool CamIsOutsideDeadzone()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 camPos = cam.transform.position;

        if (Mathf.Abs(playerPos.x - camPos.x) > 1 || Mathf.Abs(playerPos.y - camPos.y) > 1 || Mathf.Abs(playerPos.z - camPos.z) > 1)
        {
            return true;
        }
        return false;
    }
}
