using System;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    idle,
    patrol, 
    chase, 
    attack,
}

public enum BossStates
{
    peido, 
    pudim,
}

public class Enemy : MonoBehaviour
{
    private AIBrain<State> _brain;

    private bool _shouldIdle = false;
    private bool _shouldPatrol = false;
    private bool _shouldChase = false;
    private bool _shouldAttack = false;

    private void Awake()
    {
        _brain = new AIBrain<State>();
        
        _brain.AddState(State.idle, Idle);
        _brain.AddState(State.patrol, Patrol);
        _brain.AddState(State.chase, Chase);
        _brain.AddState(State.chase, Attack);

        /**/
        _brain.AddTransition(State.idle, State.chase, () => _shouldChase);
        _brain.AddTransition(State.chase, State.idle, () => !_shouldChase);

        _brain.AddTransition(State.idle, State.attack, () => _shouldAttack);
        _brain.AddTransition(State.attack, State.patrol, () => !_shouldAttack);

        /**/

        _brain.AddTransition(State.chase, State.patrol, () => _shouldPatrol);
        _brain.AddTransition(State.chase, State.attack, () => _shouldAttack);

        /**/
        _brain.AddTransition(State.chase, State.idle, () => !_shouldChase);
        _brain.AddTransition(State.chase, State.patrol, () => _shouldPatrol);
        _brain.AddTransition(State.chase, State.attack, () => _shouldAttack);

    }

    private void Update()
    {
        CheckBools();
    }

    void CheckBools()
    {
        if (_shouldIdle)
        {

        }
        else
        {

        }

        if (_shouldChase)
        {
            ;
        }
    }

    private void FixedUpdate()
    {
        _brain.Tick();
    }

    private void Idle()
    {
        Debug.Log("foda-se");
    }

    private void Patrol()
    {

    }

    private void Chase()
    {
        Debug.Log("Chasing");
    }

    private void Attack()
    {

    }

}
