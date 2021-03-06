using System;
using System.Collections;
using UnityEngine;

public class Grounder
{
    private readonly Vector3 _grounderPosition;
    private readonly Vector2 _grounderSizes;
    private readonly LayerMask _layerMask;

    private readonly Vector3 _nearGroundPosition;
    private readonly Vector2 _nearGroundSizes;
    private readonly Player _player;
    private readonly Transform _playerTransform;
    private readonly Rigidbody2D _rigidbody;

    private Coroutine _coyoteRoutine = null;

    private float _permitedArialTime;
    private bool _isGrounded;

    public static event Action OnEnterGround;
    public static event Action OnLeaveGround;

    public Grounder(Player player)
    {
        _layerMask = player.PlayerParameters.GrounderLayerMask;
        _playerTransform = player.transform;
        _grounderPosition = player.PlayerParameters.GrounderPosition;
        _grounderSizes = player.PlayerParameters.GrounderSizes;

        _nearGroundPosition = player.PlayerParameters.NearGroundGrounderPosition;
        _nearGroundSizes = player.PlayerParameters.NearGroundGrounderSizes;

        _permitedArialTime = player.PlayerParameters.PermitedArialTime;

        _rigidbody = player.GetComponent<Rigidbody2D>();

        _player = player;
    }

    public bool IsGrounded => _isGrounded;

    public bool NearGround { get; private set; }

    public bool CoyoteGround { get; private set; }

    public void Tick()
    {
        _permitedArialTime = _player.PlayerParameters.PermitedArialTime;
        var lastFrameGround = IsGrounded;
        
        UpdateGroundedValues();

        if (lastFrameGround && !IsGrounded)
        {
            _coyoteRoutine = _player.StartCoroutine(ArialTimeCounter());
            OnLeaveGround?.Invoke();
        }
        if (!lastFrameGround && IsGrounded) OnEnterGround?.Invoke();

        
        if ((_coyoteRoutine != null || CoyoteGround) && IsGrounded)
        {
            CoyoteGround = false;
            _player.StopCoroutine(_coyoteRoutine);
            _coyoteRoutine = null;
        }
    }

    private void UpdateGroundedValues()
    {
        Vector3 position = _playerTransform.position;
        _isGrounded = Physics2D.OverlapBox(position + _grounderPosition, _grounderSizes, 0f, _layerMask);
        NearGround = _rigidbody.velocity.y <= 0f &&
                     Physics2D.OverlapBox(position + _nearGroundPosition, _nearGroundSizes, 0f, _layerMask);
    }


    private IEnumerator ArialTimeCounter()
    {
        CoyoteGround = true;

        yield return new WaitForSeconds(_permitedArialTime);

        CoyoteGround = false;
    }
}
