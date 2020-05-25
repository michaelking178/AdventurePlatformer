using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private Patrol patrol;
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private StateHandler stateHandler;
    private Entity entity;
    private PlayerController player;

    private float defaultSpeed;
    private float runSpeed;
    private Vector3 currentTarget;


    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float runSpeedModifier = 1.25f;

    [SerializeField]
    private float rotationSpeed = 1f;

    [SerializeField]
    private float lineOfSight = 10f;

    [SerializeField]
    private float attackReach = 1f;

    // Start is called before the first frame update
    void Start()
    {
        patrol = GetComponent<Patrol>();
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateHandler = GetComponent<StateHandler>();
        entity = GetComponent<Entity>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        defaultSpeed = speed;
        runSpeed = speed * runSpeedModifier;
    }

    private void FixedUpdate()
    {
        WatchForPlayer();
        RotateToward(currentTarget);

        if (!stateHandler.Compare("ATTACKING"))
        {
            MoveToward(currentTarget);
        }
    }

    private void MoveToward(Vector3 targetPos)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < attackReach)
        {
            // Stop
            navMeshAgent.SetDestination(transform.position);
            anim.SetFloat("Speed", 0f);
            if (!stateHandler.Compare("ATTACKING"))
            {
                StartCoroutine(entity.Attack());
            }
        }
        else
        {
            navMeshAgent.SetDestination(targetPos);
            navMeshAgent.speed = speed;
            anim.SetFloat("Speed", speed * 0.2f);
        }
    }

    private void RotateToward(Vector3 targetPos)
    {
        Vector3 targetDir = targetPos - transform.position;
        Vector3 direction = Vector3.RotateTowards(transform.forward, targetDir, Time.fixedDeltaTime * rotationSpeed, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void WatchForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < lineOfSight)
        {
            currentTarget = player.transform.position;
            speed = runSpeed;
            
        }
        else
        {
            speed = defaultSpeed;
            currentTarget = patrol.GetNextWaypoint();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}
