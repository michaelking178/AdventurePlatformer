using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [Tooltip("Waypoint 0 should be set to the object's starting position")]
    [SerializeField] private List<Vector3> waypoints;

    [SerializeField]
    private float maxVel = 15f;
    
    [SerializeField]
    private float toVel = 2.5f;

    [SerializeField]
    private float maxForce = 40f;

    [SerializeField]
    private float gain = 5f;

    [SerializeField]
    private float antigravityBoost = 50f;

    private Rigidbody rb;

    // Waypoints
    private int current = 0;
    private int target;

    private void Start()
    {
        target = current + 1;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        AdjustForGravity();
    }

    private void Move()
    {
        Vector3 distance = waypoints[target] - transform.position;
        Vector3 tgtVel = Vector3.ClampMagnitude(toVel * distance, maxVel);
        Vector3 error = tgtVel - rb.velocity;
        Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);

        rb.AddForce(force);

        Vector2 posVector2 = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetVector2 = new Vector2(waypoints[target].x, waypoints[target].z);

        if (posVector2.Round(2) == targetVector2)
        {
            StartCoroutine(ArriveAtWaypoint());
        }
    }

    private IEnumerator ArriveAtWaypoint()
    {
        current = target;
        target++;

        if (target == waypoints.Count)
        {
            target = 0;
        }

        yield return new WaitForSeconds(1f);
    }

    private void AdjustForGravity()
    {
        float originY = waypoints[current].y;
        float targetY = waypoints[target].y;

        float totalDistance = Vector3.Distance(waypoints[current], waypoints[target]);
        float distanceTravelled = Vector3.Distance(transform.position, waypoints[current]);
        float percentTravelled = Mathf.Abs(distanceTravelled / totalDistance);

        float currentY = Mathf.Lerp(originY, targetY, percentTravelled);

        if (transform.position.y < currentY)
        {
            float yDifference = currentY - transform.position.y;
            Vector3 antiGrav = new Vector3(0f, yDifference * antigravityBoost, 0f);
            rb.AddForce(antiGrav);

            if (name == "Hover Platform Small (1)")
            {
                Debug.Log(yDifference);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector3 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, 0.5f);
            Gizmos.color = Color.cyan;
        }
    }
}
