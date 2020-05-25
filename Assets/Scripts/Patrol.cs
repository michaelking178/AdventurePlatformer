using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    private int targetWaypoint;

    [SerializeField]
    private List<Vector3> waypoints;

    private void Start()
    {
        targetWaypoint = 1;
    }

    private void Update()
    {
        Vector2 posVector2 = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetVector2 = new Vector2(waypoints[targetWaypoint].x, waypoints[targetWaypoint].z);

        if (posVector2.Round(2) == targetVector2.Round(2))
        {
            targetWaypoint++;
            if (targetWaypoint == waypoints.Count)
            {
                targetWaypoint = 0;
            }
        }
    }

    public Vector3 GetCurrentWaypoint()
    {
        if (targetWaypoint == 0)
        {
            return waypoints[waypoints.Count - 1];
        }
        else
        {
            return waypoints[targetWaypoint - 1];
        }
    }

    public Vector3 GetNextWaypoint()
    {
        return waypoints[targetWaypoint];
    }
}
