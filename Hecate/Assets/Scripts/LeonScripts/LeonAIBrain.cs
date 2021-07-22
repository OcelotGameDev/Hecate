using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeonAIBrain
{
    public Action currentAction;
    public void Brain(Action newAction)
    {
        currentAction = newAction;
        currentAction.Invoke();
    }
}

public interface IHittable
{
    void Hit(int damage = 1);
}
