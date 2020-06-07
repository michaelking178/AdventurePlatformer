using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{

    [SerializeReference]
    GameObject user;

    [SerializeField]
    private int damage = 2;
    public int Damage()
    {
        return damage;
    }

    [SerializeField]
    private ParticleSystem hitEffect;
    public ParticleSystem HitEffect()
    {
        return hitEffect;
    }

    private StateHandler userStateHandler;
    private List<string> collisionTags;

    private GameObject target;
    public GameObject Target()
    {
        return target;
    }

    private bool attackContact;
    public bool AttackContact()
    {
        return attackContact;
    }

    private void Start()
    {
        collisionTags = new List<string>();

        if (user != null)
        {
            userStateHandler = user.GetComponent<StateHandler>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (userStateHandler == null || !userStateHandler.Compare("ATTACKING"))
        {
            return;
        }

        if (!collisionTags.Contains(transform.root.tag))
        {
            collisionTags.Add(transform.root.tag);
        }
        if (!collisionTags.Contains(other.tag))
        {
            collisionTags.Add(other.tag);
        }

        if (collisionTags.Contains("Player") && collisionTags.Contains("Enemy"))
        {
            target = other.gameObject;
            attackContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        attackContact = false;
        collisionTags.Clear();
    }

    public void SetUser(GameObject _user)
    {
        user = _user;
    }
}
