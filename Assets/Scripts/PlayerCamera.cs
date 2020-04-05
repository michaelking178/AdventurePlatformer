using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;
    private float defaultDistanceToCam;
    private float distanceToCam;
    private LayerMask mask;
    private RaycastHit hit;

    [SerializeField] private float lerpDelta = 0.1f;
    [SerializeField] private Vector3 cameraZoomOffset = new Vector3();

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        defaultDistanceToCam = Vector3.Distance(transform.position, cam.transform.position);
        mask = LayerMask.GetMask("NotClippable");
    }

    void FixedUpdate()
    {
        Vector3 direction = cam.transform.position - transform.position;
        distanceToCam = Vector3.Distance(transform.position, cam.transform.position);

        if (Physics.SphereCast(transform.position, 1f, direction, out hit, distanceToCam, mask))
        {
            float distanceToCollision = Vector3.Distance(transform.position, hit.point);

            if (distanceToCam >= distanceToCollision)
            {                
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, transform.position + cameraZoomOffset, lerpDelta);
                distanceToCam = Vector3.Distance(transform.position, cam.transform.position);
            }
        }
        else
        {
            ResetCamera(direction);
        }
    }

    private void ResetCamera(Vector3 direction)
    {
        if (distanceToCam < defaultDistanceToCam)
        {
            Vector3 resetPos = cam.transform.position - cameraZoomOffset + direction;
            Vector3 newCamPos = Vector3.Lerp(cam.transform.position, resetPos, lerpDelta);
            cam.transform.position = newCamPos;
        }
    }
}
