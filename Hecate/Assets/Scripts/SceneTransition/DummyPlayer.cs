using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SceneTransition
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DummyPlayer : MonoBehaviour
    {
        [SerializeField] 
        private float _moveSpeed = 10f;

        private PlayerActions _playerActions;

        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _playerActions = new PlayerActions();
            _playerActions.Default.Enable();

            _rigidbody = this.GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            var input = _playerActions.Default.Move.ReadValue<Vector2>();

            _rigidbody.velocity = input * _moveSpeed;
        }
    }
}