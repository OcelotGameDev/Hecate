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

    /*public void SetMonsterType()
    {
        switch (monsterType)
        {
            case TypesOfMonster.Lobisomen:
                {
                    maxSpeed = 2;
                    maxHp = 5;
                    sightRadius = 3;
                    sightAngle = 35;
                    aiAction = SightAI;
                }
                break;

            case TypesOfMonster.Cachorro:
                {
                    maxSpeed = 5;
                    maxHp = 10;
                    sightRadius = 4;
                    sightAngle = 35;
                    aiAction = SightAI;
                    canDash = true;
                }
                break;

            case TypesOfMonster.Boss:
                {
                    maxSpeed = 15;
                    maxHp = 20;
                    sightRadius = 6;
                    sightAngle = 35;
                    aiAction = SightAI;
                    canDash = true;
                }
                break;
        }
    }*/
}