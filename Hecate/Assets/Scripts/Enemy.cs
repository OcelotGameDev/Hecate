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

    private bool _shoudlChase = false;

    private void Awake()
    {
        _brain = new AIBrain<State>();
        
        _brain.AddState(State.idle, Idle);
        _brain.AddState(State.chase, Chase);
        
        _brain.AddTransition(State.idle, State.chase, () => _shoudlChase );
        _brain.AddTransition(State.chase, State.idle, () => !_shoudlChase );
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _shoudlChase = true;
        }
        else
        {
            _shoudlChase = false;
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

    private void Chase()
    {
        Debug.Log("Chasing");
    }
}
