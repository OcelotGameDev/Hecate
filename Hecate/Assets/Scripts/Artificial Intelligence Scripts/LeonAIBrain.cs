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

    IEnumerator Timer(Action action, float waitTime) //teste de ideia de melhor automa��o de transi��o.
    {
        yield return new WaitForSeconds(waitTime);
        currentAction = action;
    }
}