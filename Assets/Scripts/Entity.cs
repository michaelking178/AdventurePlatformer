using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private StateHandler stateHandler;
    private GearHandler gearHandler;
    private Animator anim;
    private Gear gear;

    [SerializeField]
    private int hitpoints = 10;

    [SerializeField]
    private float attackTime = 0.5f;

    private void Start()
    {
        stateHandler = GetComponent<StateHandler>();
        gearHandler = GetComponent<GearHandler>();
        anim = GetComponent<Animator>();
        gear = gearHandler.GetCurrentGear();
    }

    private void Update()
    {
        if (hitpoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator Attack()
    {
        if (stateHandler.Compare("ATTACKING"))
        {
            yield return null;
        }

        if (stateHandler.Compare("GROUNDED"))
        {
            anim.SetTrigger("Attack");
            // The AttackHit() method is called by the animation
        }
        else if (stateHandler.Compare("JUMPING") || stateHandler.Compare("FALLING"))
        {
            anim.SetTrigger("Jump Attack");
        }

        stateHandler.SetState("ATTACKING");
        yield return new WaitForSeconds(attackTime);
        stateHandler.Revert();
    }

    private void ApplyAttack()
    {
        if (gear.AttackContact())
        {
            gear.Target().GetComponent<Entity>().ReceiveDamage(gear.Damage());
            Instantiate(gear.HitEffect(), transform.position, Quaternion.identity);
        }
    }

    public void ReceiveDamage(int damage)
    {
        hitpoints -= damage;
    }
}
