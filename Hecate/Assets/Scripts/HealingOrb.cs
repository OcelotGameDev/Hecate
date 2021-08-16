using System;
using UnityEngine;

public class HealingOrb : MonoBehaviour
{
    [SerializeField] private int _healAmount = 1;
    [SerializeField] private float _moveForce = 2;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Player.PlayerPosition != null)
        {
            _rigidbody.AddForce((Player.PlayerPosition.position - this.transform.position).normalized * _moveForce);        
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();

        if (player)
        {
            player.Heal(_healAmount);
            Destroy(this.gameObject);
        }
    }
}
