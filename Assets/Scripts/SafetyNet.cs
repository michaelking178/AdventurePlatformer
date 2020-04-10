using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyNet : MonoBehaviour
{
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 startingRotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = startingPosition;
            other.transform.rotation = Quaternion.Euler(startingRotation);
        }
    }
}
