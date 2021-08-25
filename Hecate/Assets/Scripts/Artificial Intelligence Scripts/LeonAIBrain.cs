using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonAIBrain
{
    Action currentAction;

    public void Brain(Action newAction)
    {
        currentAction = newAction;
        currentAction.Invoke();
    }
}