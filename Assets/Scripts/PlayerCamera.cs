﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;
    private Vector3 camLocalPos;
    private float defaultDistanceToCam;
    private float distanceToCam;
    private LayerMask mask;
    private RaycastHit hit;

    [SerializeField] private float lerpDelta = 0.1f;
    [SerializeField] private Vector3 cameraZoomOffset = new Vector3();

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        camLocalPos = cam.transform.localPosition;
        defaultDistanceToCam = Vector3.Distance(transform.position, cam.transform.position);
        mask = LayerMask.GetMask("NotClippable");
    }

    void FixedUpdate()
    {
        Vector3 direction = cam.transform.position - transform.position;
        distanceToCam = Vector3.Distance(transform.position, cam.transform.position);

        if (Physics.SphereCast(transform.position, 0.5f, direction, out hit, distanceToCam, mask))
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
        if (distanceToCam < defaultDistanceToCam && CamIsOutsideDeadzone())
        {
            Vector3 newCamPos = Vector3.Lerp(cam.transform.localPosition, camLocalPos, lerpDelta);
            cam.transform.localPosition = newCamPos;
        }
    }

    private bool CamIsOutsideDeadzone()
    {
        Vector3 playerPos = transform.position;
        Vector3 camPos = cam.transform.position;

        if (Mathf.Abs(playerPos.x - camPos.x) > 1 || Mathf.Abs(playerPos.y - camPos.y) > 1 || Mathf.Abs(playerPos.z - camPos.z) > 1)
        {
            return true;
        }
        return false;
    }
}
