using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine: MonoBehaviour
{
    protected States currentState;

    public void SetState(States state)
    {
        currentState = state;
        StartCoroutine(currentState.Patrol());
    }
}
