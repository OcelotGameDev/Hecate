using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _timeUntilDespawn = 10f;
    private float _timer; 
    
    private Rigidbody2D _body;

    private void Awake()
    {
        _body = this.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _timer = _timeUntilDespawn;
    } 
    
    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            Destroy(gameObject);
        }
        _body.velocity = this.transform.right * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var hitObject = other.GetComponent<IHittable>();
        hitObject?.Hit();
        
        OnHitSomething();
    }

    private void OnBecameInvisible()
    {
        OnHitSomething();
    }

    private void OnHitSomething()
    {
        // Instantiate explosion Effect
        // Play Explosion Animation

        Destroy(this.gameObject);
    }
    
    private void OnValidate()
    {
        if (this.gameObject.layer != LayerMask.NameToLayer("PlayerBullets"))
            this.gameObject.layer = LayerMask.NameToLayer("PlayerBullets");
    }
}