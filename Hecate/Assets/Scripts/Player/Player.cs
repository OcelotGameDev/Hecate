using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _body;
    
    private Mover _mover;

    private PlayerActions _actions;

    [Header("Params")] 
    [SerializeField] private float _moveSpeed = 6;
    [SerializeField] private float _moveAcceleration = 10;

    private void Awake()
    {
        _actions = new PlayerActions();
        _actions.Default.Enable();

        _mover = new Mover(_actions.Default.Move, _body, _moveSpeed, _moveAcceleration);
    }

    private void OnValidate()
    {
        if (!_body) _body = this.GetComponent<Rigidbody2D>();
    }
}

public class Mover
{
    private readonly InputAction _moveAction;
    private readonly Rigidbody2D _body;
    private readonly float _moveSpeed;
    private readonly float _moveAcceleration;

    public Mover(InputAction moveAction, Rigidbody2D body, float moveSpeed, float moveAcceleration)
    {
        _moveAction = moveAction;
        _body = body;
        _moveSpeed = moveSpeed;
        _moveAcceleration = moveAcceleration;
    }

    public void Tick()
    {
        
    }
}
