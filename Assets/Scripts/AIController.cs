using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    [SerializeReference]
    Vector3 moveDirection;

    private Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        StartCoroutine(ChooseMoveDirection());
    }

    // Update is called once per frame
    void Update()
    {
        movement.Move(moveDirection);
        Debug.Log(moveDirection);
    }

    private IEnumerator ChooseMoveDirection()
    {
        while (true)
        {
            int dirX = Random.Range(0, 2);
            int dirZ = Random.Range(0, 2);

            moveDirection = new Vector3(dirX, 0f, dirZ);

            yield return new WaitForSeconds(3f);
        }
    }
}
