using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypesOfBhvr { Idle, Patrol, Chase, Attack };

public class LeonAIBrain
{
    Action currentAction;
    TypesOfBhvr enumBehaviours;

    public void Brain(Action newAction)
    {
        currentAction = newAction;
        currentAction.Invoke();
    }
}