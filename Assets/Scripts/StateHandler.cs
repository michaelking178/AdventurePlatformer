using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    public enum State
    {
        GROUNDED,
        JUMPING,
        FALLING,
        ATTACKING
    };

    [SerializeReference]
    private State state;

    private State previousState;
    private Animator anim;
    private float previousHeight;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        switch (state)
        {
            case State.GROUNDED:
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                break;

            case State.JUMPING:
                anim.SetBool("isGrounded", false);
                anim.SetBool("isFalling", false);
                if (ReachedJumpPeak())
                {
                    SetState(State.FALLING);
                }
                break;

            case State.FALLING:
                anim.SetBool("isFalling", true);
                anim.SetBool("isGrounded", false);
                break;

            case State.ATTACKING:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Sets the State Handler's current state to the State provided
    /// </summary>
    public void SetState(State newState)
    {
        if (previousState != newState)
        {
            previousState = state;
        }
        state = newState;
    }

    /// <summary>
    /// Sets the State Handler's current state to the State name provided. State name must match an existing State enum key.
    /// </summary>
    public void SetState(string newState)
    {
        State stateToSet = (State)System.Enum.Parse(typeof(State), newState);
        if (previousState != stateToSet)
        {
            previousState = state;
        }
        state = stateToSet;
    }

    /// <summary>
    /// Returns the State Handler's current state
    /// </summary>
    public State GetState()
    {
        return state;
    }

    /// <summary>
    /// Returns the State Handler's previous State.
    /// </summary>
    public State GetPreviousState()
    {
        return previousState;
    }

    /// <summary>
    /// Compares the State Handler's current state to the state name provided. State name must match an existing State enum key.
    /// </summary>
    public bool Compare(string stateName)
    {
        State stateToCompare = (State)System.Enum.Parse(typeof(State), stateName);

        if (state != stateToCompare)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Compares the State Handler's current state to the State provided
    /// </summary>
    public bool Compare(State comparingState)
    {
        if (state != comparingState)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool ReachedJumpPeak()
    {
        float currentHeight = transform.position.y;
        if (currentHeight - previousHeight < -0f)
        {
            return true;
        }
        previousHeight = currentHeight;
        return false;
    }
}
