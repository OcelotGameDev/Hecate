using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Jumper : IJumper
{
    private readonly float _gravityFallMultiplayer;
    private readonly float _jumpVelocity;
    private readonly float _maxButtonHoldTime;
    private readonly Player _player;
    private readonly Grounder _playerGrounder;

    private bool _canJumpAgain = true;
    private float _jumpTimer;
    private readonly Rigidbody2D _rigidbody;
    private Vector2 _velocityWhenTerminateJump;

    public Jumper(Player player)
    {
        _player = player;
        _rigidbody = player.gameObject.GetComponent<Rigidbody2D>();
        _playerGrounder = player.Grounder;
        _jumpVelocity = player.PlayerParameters.JumpVelocity;
        _maxButtonHoldTime = player.PlayerParameters.MaxJumpHoldTime;
        _gravityFallMultiplayer = player.PlayerParameters.GravityFallMultiplayer;
        _velocityWhenTerminateJump = new Vector2(0f, player.PlayerParameters.VelocityWhenTerminateJump);
    }

    public bool Jumping { get; private set; }

    public void Tick()
    {
        Vector2 velocity = _rigidbody.velocity;

        var timerFactor = 1 - _jumpTimer / _maxButtonHoldTime;

        //if (RewiredPlayerInput.Instance.Jump)
        {
            if (_playerGrounder.IsGrounded && _canJumpAgain || Jumping)
            {
                if (_jumpTimer <= _maxButtonHoldTime)
                {
                    velocity.y = _jumpVelocity * timerFactor;
                    Jumping = true;
                    _jumpTimer += Time.deltaTime;
                    _canJumpAgain = false;
                }
            }
        }
       //  else
       //  {
       //      _jumpTimer = 0f;
       //      Jumping = false;
       //  }

        _rigidbody.gravityScale = velocity.y < -0.1f ? _gravityFallMultiplayer : 1f;
        
        //if (RewiredPlayerInput.Instance.TerminateJump)
        {
            if (_rigidbody.velocity.y > _jumpVelocity / 2f)
            {
                _velocityWhenTerminateJump.x = _rigidbody.velocity.x;
                velocity = _velocityWhenTerminateJump;
            }

            _canJumpAgain = true;
        }

        //if (RewiredPlayerInput.Instance.InitiateJump)
        {
            // If near ground Can jump, otherwise can't
            _canJumpAgain = _playerGrounder.NearGround;
        }

        _rigidbody.velocity = velocity;
    }
}

public class NewJumper : IJumper
{
    private readonly Player _player;
    private readonly Grounder _playerGrounder;

    private bool _canJumpAgain = true;
    private float _gravityFallMultiplayer;

    private bool _jumpAction;
    private float _jumpTimer;
    private float _jumpVelocity;
    private readonly float _lowGravityFallMultiplayer;
    private float _permitedArialTime;
    private readonly Rigidbody2D _rigidbody;

    private bool _startedJump = false;
    private bool _jumping = false;
    private bool _endedJump = false;

    public NewJumper(Player player)
    {
        _player = player;
        _rigidbody = player.gameObject.GetComponent<Rigidbody2D>();
        _playerGrounder = player.Grounder;
        _jumpVelocity = player.PlayerParameters.JumpVelocity;
        _gravityFallMultiplayer = player.PlayerParameters.GravityFallMultiplayer;
        _lowGravityFallMultiplayer = player.PlayerParameters.LowGravityFallMultiplayer;
        _permitedArialTime = player.PlayerParameters.PermitedArialTime;

        _player.PlayerInputs.Default.Jump.started += JumpStart;
        _player.PlayerInputs.Default.Jump.canceled += JumpEnd;
    }

    ~NewJumper()
    {
        _player.PlayerInputs.Default.Jump.started -= JumpStart;
        _player.PlayerInputs.Default.Jump.canceled -= JumpEnd;
    }
    

    public bool Jumping { get; }

    public void Tick()
    {
        // Debug.Log(_player.Grounder.CoyoteGround);
        // Debug.Log($"{_startedJump} {_jump} {_endedJump}");
        
        // _jumpVelocity = _player.PlayerParameters.JumpVelocity;
        // _gravityFallMultiplayer = _player.PlayerParameters.GravityFallMultiplayer;
        // _permitedArialTime = _player.PlayerParameters.PermitedArialTime;
        //
        // if (_startedJump /*RewiredPlayerInput.Instance.InitiateJump*/ &&
        //     (_playerGrounder.IsGrounded || _playerGrounder.NearGround || _playerGrounder.CoyoteGround) &&
        //     !_jumpAction)
        // {
        //     JumpAction(Vector2.up);
        // }

        if ((!_playerGrounder.IsGrounded) && (_rigidbody.velocity.y < 0 || !_jumping))
        {
            _rigidbody.gravityScale = 10;
            // _rigidbody.velocity +=
            //     Vector2.up * (Physics2D.gravity.y * (_gravityFallMultiplayer - 1) * Time.deltaTime);
        }
        else
        {
            _rigidbody.gravityScale = 1;
        }
        
        // else if (_rigidbody.velocity.y > 0 && !_jump /*RewiredPlayerInput.Instance.Jump*/)
        // {
        //     _rigidbody.velocity +=
        //         Vector2.up * (Physics2D.gravity.y * (_lowGravityFallMultiplayer - 1) * Time.deltaTime);
        // }
    }
    
    private void JumpStart(InputAction.CallbackContext context)
    {
        if (!(_player.Grounder.IsGrounded || _player.Grounder.NearGround || _player.Grounder.CoyoteGround)) return;
        
        JumpAction(Vector2.up);
        _jumping = true; 
    }

    private void JumpEnd(InputAction.CallbackContext context)
    {
        _jumping = false;
    }

    private void JumpAction(Vector2 jumpDirection)
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity = new Vector2(velocity.x, 0f);
        velocity += jumpDirection * _jumpVelocity;
        _rigidbody.velocity = velocity;

        // Pooler.Instance.SpawnFromPool(_playerGrounder.CoyoteGround ? "AirJump" : "GroundJump",
        //     _player.transform.position, Quaternion.identity);

        _jumpAction = true;
        _player.StartCoroutine(FinishJumpAction());
    }

    private IEnumerator FinishJumpAction()
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() =>
            _playerGrounder.IsGrounded || _playerGrounder.NearGround 
                                       || _endedJump /*RewiredPlayerInput.Instance.TerminateJump*/ 
                                       || !_jumping /*RewiredPlayerInput.Instance.Jump*/);
        _jumpAction = false;
    }
}
