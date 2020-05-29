using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private StateHandler stateHandler;
    private Animator anim;

    [SerializeField]
    private int hitpoints = 10;

    [SerializeField]
    private float attackTime = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        stateHandler = GetComponent<StateHandler>();
        anim = GetComponent<Animator>();
    }

    public IEnumerator Attack()
    {
        if (!stateHandler.Compare("ATTACKING"))
        {
            if (stateHandler.Compare("GROUNDED")) // Must be grounded, and cannot attack while already attacking!
            {
                anim.SetTrigger("Attack");
                stateHandler.SetState("ATTACKING");
                yield return new WaitForSeconds(attackTime);
                stateHandler.Revert();
            }
            else if (stateHandler.Compare("JUMPING") || stateHandler.Compare("FALLING"))
            {
                anim.SetTrigger("Jump Attack");
                stateHandler.SetState("ATTACKING");
                yield return new WaitForSeconds(attackTime);
                stateHandler.Revert();
            }
        }
    }

    public void ReceiveDamage(int damage)
    {
        hitpoints -= damage;
        Debug.Log("Hitpoints: " + hitpoints);
    }
}
