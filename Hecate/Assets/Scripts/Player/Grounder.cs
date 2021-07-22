using UnityEngine;

public class Grounder : MonoBehaviour
{
    [SerializeField] private Vector3 _grounderPosition;
    [SerializeField] private Vector2 _grounderSizes;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private Vector3 _nearGroundPosition;
    [SerializeField] private Vector2 _nearGroundSizes;
    [SerializeField] private Player _player;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Rigidbody2D _rigidbody;
    
    [SerializeField] private float _permitedArialTime;
    private bool _isGrounded;
}