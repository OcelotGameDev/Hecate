using System;
using System.Collections;
using System.Linq;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;


public enum Directions
{
    Right = 1,
    Up,
    Left,
    Down
}

[RequireComponent(typeof(PlayerAnimatorController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour, IHittable
{
    // [SerializeField] private PlayParticlesGeneral _hurtParticles;

    [SerializeField]
    [AssetsOnly]
    [FoldoutGroup("Player Parameters")]
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    private PlayerParameters _playerParameters;

    [SerializeField] private PlayerAnimatorController _playerAnimatorController;

    // public static RoomManager CurrentRoomManager = null;

    public bool CanMove = true;

    [SerializeField] private PlayerGhostSpawner _ghostSpawner;
    private IAttacker _attacker;
    private bool _attacking;
    private bool _canDash = true;
    private Glider _glider;

    private IJumper _jumper;
    private bool _justTouchedGround;

    private bool _lastFrameJumpState;

    //private List<IAttacker> _attackerList;
    private IMover _mover;
    private PlayerLocker _playerLocker;
    private WaitForSeconds _waitForSecondsDash;
    //private WallJumper _wallJumper;
    
    private AAimSystem _aimSystem;
    private string _bindingGroup;
    
    public bool IsDashing => _mover is Dasher;

    public PlayerParameters PlayerParameters => _playerParameters;
    public LifeSystem LifeSystem { get; private set; }

    public Grounder Grounder { get; private set; }

    public bool Jumping => _jumper.Jumping;
    public bool Gliding => _glider.Gliding;
    public PlayerAnimatorController AnimatorController => _playerAnimatorController;

    private Coroutine _invincibilityCoroutine = null;

    public Rigidbody2D Rigidbody { get; private set; }
    public bool IsAttacking => _attacking;

    public PlayerActions PlayerInputs;

    public static Transform PlayerPosition;
    
    [Header("Parameters")]
    [SerializeField] private float _aimDistance = 2f;
    [SerializeField] private float _castCadence = 0.5f;
    [SerializeField] private GameObject _bulletPrefab;
    
    [Header("Objects")] 
    [SerializeField] private Transform _aim;
    [SerializeField] private Transform _aimParent;
    [SerializeField] private Transform _castingPoint;
    
    private CastingSystem _castingSystem;

    private void Awake()
    {
        PlayerInputs = new PlayerActions();
        PlayerInputs.Default.Enable();

        AttackerTimer.SetTimer(_playerParameters.TimeBetweenAttacks);
        _waitForSecondsDash = new WaitForSeconds(_playerParameters.TimeBetweenDashes);

        Grounder = new Grounder(this);
        LifeSystem = new LifeSystem(this);
        _mover = new NewMover(this);
        _jumper = new NewJumper(this);
        _playerLocker = new PlayerLocker(this);
        _glider = new Glider(this);
        _attacker = new BasicAttacker(this);
        //_wallJumper = new WallJumper(this);
        _castingSystem = new CastingSystem(PlayerInputs.Default.Shoot, _castCadence, _bulletPrefab, _castingPoint.transform);

        Rigidbody = this.GetComponent<Rigidbody2D>();

        // _attackerList = new List<IAttacker>();
        // _attackerList.Add(new BasicAttacker(this));
        // _attackerList.Add(new StrongAttacker(this));
    }

    private void Update()
    {
        Grounder.Tick();
        
        _mover.Tick();
        
        _jumper.Tick();
        
        _aimSystem.Tick();
        _castingSystem.Tick(Time.deltaTime);

        // AttackerTimer.SubtractTimer();
        //
        // Grounder.Tick();
        //
        // if (!(_mover is ForceMover))
        // {
        //     if (!_attacking && CanMove)
        //     {
        //         _mover.Tick();
        //
        //         if (!(_mover is Dasher))
        //         {
        //             _jumper.Tick();
        //             // _wallJumper.Tick();
        //         }
        //
        //         if (Dasher.CheckDashInput()) StartDash();
        //     }
        // }
        // else
        // {
        //     _mover.Tick();
        // }
        //
        // // _attacker.Tick();
        // //_attackerList.ForEach(attacker => attacker.Tick());
        //
        // CheckJumping();
        //
        // var isGrounded = Grounder.IsGrounded;
        // // AnimatorController.UpdateParameters(isGrounded);
        // JustTouchedGround(isGrounded);
    }

    private void OnEnable()
    {
        PlayerPosition = this.transform;
        
        Grounder = new Grounder(this);
        _mover = new NewMover(this);
        // _jumper = new NewJumper(this);
        _playerLocker = new PlayerLocker(this);
        _glider = new Glider(this);
        _attacker = new BasicAttacker(this);
        
        Rigidbody.drag = PlayerParameters.BaseDrag;
        _canDash = true;
        
        Grounder.Tick();
        
        ChangeControllerScheme(ControllerChooser.CurrentScheme);
        ControllerChooser.OnSchemeChanged += ChangeControllerScheme;
    }

    private void OnDisable()
    {
        PlayerPosition = null;
        
        ControllerChooser.OnSchemeChanged -= ChangeControllerScheme;
    }
    
    private void ChangeControllerScheme(SchemeType newScheme)
    {
        if (newScheme == SchemeType.Gamepad)
        {
            _aimSystem = new AimSystemController(PlayerInputs.Default.Move, PlayerInputs.Default.Aim, _aimParent.transform, _aim.transform, _aimDistance);
            _bindingGroup = PlayerInputs.controlSchemes.First(input => input.name == "GamePad").bindingGroup;
        }
        else
        {
            _aimSystem = new AimSystemMouse(PlayerInputs.Default.Aim, _aimParent.transform, _aim.transform, Camera.main);
            _bindingGroup = PlayerInputs.controlSchemes.First(input => input.name == "Keyboard").bindingGroup;
        }
        
        // _playerInputs.Default.Aim.bindingMask = InputBinding.MaskByGroup(bindingGroup);
        PlayerInputs.bindingMask = InputBinding.MaskByGroup(_bindingGroup);
    }

    private void OnValidate()
    {
        if (_playerParameters == null)
            _playerParameters = Resources.Load<PlayerParameters>("ScriptableObjects/DefaultPlayerParameters");

        if (AnimatorController == null)
            _playerAnimatorController = GetComponent<PlayerAnimatorController>();

        if (_ghostSpawner == null)
            _ghostSpawner = GetComponent<PlayerGhostSpawner>();
    }

    public void Hit(int damage, Transform attacker)
    {
        // _hurtParticles.EmitParticle();
        _playerAnimatorController.Hurt();
        LifeSystem.Damage(damage);

        if (attacker != null)
        {
            var forceAux = (this.transform.position - attacker.position).normalized;
            if (forceAux.y < 0.2f)
            {
                forceAux += (Vector3.up * 1f);
            }

            forceAux = forceAux.normalized * _playerParameters.KnockBackForce;
            PushPlayer(forceAux);
        }

        CameraShakeShake.Instance.ShakeCamera(5f,1f);
    }
    
    public void PushPlayer(Vector3 force)
    {
        _mover = new ForceMover(this, force);
    }

    public void PushPlayer(Vector3 force, float drag)
    {
        _mover = new ForceMover(this, force, drag);
    }
    
    public void ReturnToBaseMover()
    {
        _mover = new NewMover(this);
    }

    public void OnObjectSpawn(object[] parameters = null)
    {
        if (!LifeSystem.StillAlive)
            LifeSystem = new LifeSystem(this);

        OnPlayerHeal?.Invoke(LifeSystem.CurrentLife, LifeSystem.MaxHealth);

        OnPlayerSpawn?.Invoke(gameObject);
        gameObject.SetActive(true);
    }

    public event Action OnJump;
    public event Action OnTouchedGround;

    public static event Action<GameObject> OnPlayerSpawn;
    public static event Action OnPlayerDeath;
    public static event Action<int, int> OnPlayerDamage;

    public static event Action<int, int> OnPlayerHeal;

    public static event Action<bool> OnPlayerInvincibilityChange;

    private void StartDash()
    {
        if (_canDash)
        {
            _mover = new Dasher(this);
            _playerAnimatorController.Dash();
            _ghostSpawner.StartSpawning();
            _canDash = false;
        }
    }

    public void StopDashing()
    {
        if (_mover is Dasher dasher) dasher?.StopDashing();
    }

    public void EndDash()
    {
        _mover = new NewMover(this);
        StartCoroutine(DashTimer());
    }

    private IEnumerator DashTimer()
    {
        yield return _waitForSecondsDash;
        _canDash = true;
    }

    //public void CallAttack<T>() where T : IAttacker
    public void CallAttack()
    {
        _attacker.Attack();
        //_attackerList.Find(item => item is T)?.Attack();
    }

    public void StartAttack()
    {
        _attacking = true;
        //_playerLocker.LockPlayer();
    }

    public void EndAttack()
    {
        _attacking = false;
        //_playerLocker.UnlockPlayer();
        AttackerTimer.ResetTimer();
    }

    public Directions GetAttackDirection()
    {
        // var vertical = RewiredPlayerInput.Instance.Vertical;
        var vertical = 0f;
        Directions direction;

        if (vertical > 0.25f)
        {
            direction = Directions.Up;
        }
        else if (vertical < -0.25f && !Grounder.IsGrounded)
        {
            direction = Directions.Down;
        }
        else
        {
            if (_playerAnimatorController.IsLookingRight)
                direction = Directions.Right;
            else
                direction = Directions.Left;
        }

        return direction;
    }

    public Directions GetDirectionHorizontal() => _playerAnimatorController.IsLookingRight ? Directions.Right : Directions.Left;

    private void CheckJumping()
    {
        if (!_lastFrameJumpState && _jumper.Jumping) OnJump?.Invoke();

        _lastFrameJumpState = _jumper.Jumping;
    }

    private void JustTouchedGround(bool isGrounded)
    {
        if (!_justTouchedGround && isGrounded) OnTouchedGround?.Invoke();

        _justTouchedGround = isGrounded;
    }

    public void Died() => OnPlayerDeath?.Invoke();

    public void DamageDealt(int currentLife, int maxLife)
    {
        OnPlayerDamage?.Invoke(currentLife, maxLife);
        
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
        }

        StartCoroutine(InvincibilityTime(LifeSystem.InvincibilityTime));
    }

    public void Heal(int healAmount)
    {
        LifeSystem.Heal(healAmount);
    }

    [Button] public void Damage() => Hit(1, null);

    private IEnumerator InvincibilityTime(float invincibleTime)
    {
        LifeSystem.SetInvincible(true);
        OnPlayerInvincibilityChange?.Invoke(true);
        
        yield return new WaitForSeconds(invincibleTime);
        
        LifeSystem.SetInvincible(false);
        OnPlayerInvincibilityChange?.Invoke(false);
    }
    
    

#if UNITY_EDITOR
    [SerializeField] [EnumToggleButtons] private GizmosType _gizmosToShow = 0;
    

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;

        foreach (GizmosType gizmo in Enum.GetValues(typeof(GizmosType)))
            if ((_gizmosToShow & gizmo) == gizmo)
                switch (gizmo)
                {
                    case GizmosType.Grounder:
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(position + _playerParameters.GrounderPosition,
                            _playerParameters.GrounderSizes);
                        break;
                    case GizmosType.NearGround:
                        Gizmos.color = new Color(0f, 0.5f, 0f, 1f);
                        Gizmos.DrawWireCube(position + _playerParameters.NearGroundGrounderPosition,
                            _playerParameters.NearGroundGrounderSizes);
                        break;
                    case GizmosType.BasicAttack:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(position + (Vector3) _playerParameters.BasicAttackCenter,
                            _playerParameters.BasicAttackRadius);
                        Vector3 leftAttackCenter = (Vector3) _playerParameters.BasicAttackCenter;
                        leftAttackCenter.x *= -1;
                        Gizmos.DrawWireSphere(position + leftAttackCenter, _playerParameters.BasicAttackRadius);
                        break;
                    case GizmosType.BasicAttackUp:
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawWireSphere(position + (Vector3) _playerParameters.BasicUpAttackCenter,
                            _playerParameters.BasicUpAttackRadius);
                        break;
                    case GizmosType.BasicAttackDown:
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(position + (Vector3) _playerParameters.BasicDownAttackCenter,
                            _playerParameters.BasicDownAttackRadius);
                        break;
                    case GizmosType.WallChecker:
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(position + (Vector3) _playerParameters.WallJumpBoxOffsetRight,
                            _playerParameters.WallJumpBoxSizes);
                        Gizmos.DrawWireCube(position + (Vector3) _playerParameters.WallJumpBoxOffsetLeft,
                            _playerParameters.WallJumpBoxSizes);

                        break;
                }
    }

    [Flags]
    public enum GizmosType
    {
        Grounder = 1 << 0,
        NearGround = 1 << 1,
        BasicAttack = 1 << 2,
        BasicAttackUp = 1 << 3,
        BasicAttackDown = 1 << 4,
        WallChecker = 1 << 5
    }
#endif //UNITY_EDITOR
    
}



public abstract class AAimSystem
{
    public abstract void Tick();
    
    protected static float GetDeltaAngle(Vector2 aimDirection, Quaternion currentRotation)
    {
        var newAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        var oldAngle = currentRotation.eulerAngles.z;
        return newAngle - oldAngle;
    }
}

public class AimSystemMouse : AAimSystem
{
    private readonly InputAction _aimAction;
    private readonly Transform _aimPivot;
    private readonly Transform _aim;
    private readonly Camera _camera;

    public AimSystemMouse(InputAction aimAction, Transform aimPivot, Transform aim, Camera camera)
    {
        _aimAction = aimAction;
        _aimPivot = aimPivot;
        _aim = aim;
        _camera = camera;
    }

    public override void Tick()
    {
        var mousePos = _aimAction.ReadValue<Vector2>();
        mousePos = (Vector2) _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 20));

        var dir = mousePos - (Vector2)_aimPivot.position;

        _aimPivot.Rotate(0, 0, GetDeltaAngle(dir, _aimPivot.rotation));
        
        _aim.position = mousePos;
    }
}

public class AimSystemController : AAimSystem
{
    private readonly InputAction _moveAction;
    private readonly InputAction _aimAction;
    private readonly Transform _aimPivot;

    public AimSystemController(InputAction moveAction, InputAction aimAction, Transform aimPivot, Transform aim = null, float aimDistance = -1)
    {
        _moveAction = moveAction;
        _aimAction = aimAction;
        _aimPivot = aimPivot;

        if (!aim) return;
        
        aim.localPosition = Vector3.right * aimDistance;
    }

    public override void Tick()
    {
        var couldRotate = TryRotate(_aimAction.ReadValue<Vector2>());

        if (couldRotate) return;

        TryRotate(_moveAction.ReadValue<Vector2>());
    }

    private bool TryRotate(Vector2 dir)
    {
        if (dir.magnitude < 0.2f)
            return false;
        
        _aimPivot.Rotate(0, 0, GetDeltaAngle(dir, _aimPivot.rotation)); 
        return true;
    }
}


public class CastingSystem
{
    private readonly InputAction _castAction;
    private readonly GameObject _prefab;
    private readonly Transform _castingPoint;

    private bool _casting = false;
    private bool _canCast = false;

    private readonly float _castCadence;
    private float _timer;

    public CastingSystem(InputAction castAction, float castCadence, GameObject prefab, Transform castingPoint)
    {
        _castAction = castAction;
        _castCadence = castCadence;
        _prefab = prefab;
        _castingPoint = castingPoint;

        _castAction.started += StartedCasting;
        _castAction.canceled += EndCast;
    }

    ~CastingSystem()
    {
        _castAction.started -= StartedCasting;
        _castAction.canceled -= EndCast;
    }

    public void Tick(float deltaTime)
    {
        _canCast = _timer <= 0;

        if (!_canCast)
            _timer -= deltaTime;
        else
            _timer = _castCadence;
        
        if (_casting && _canCast)
        {
            Cast();    
        }
    }

    private void Cast()
    {
        var bullet = Object.Instantiate(_prefab).transform;
        bullet.position = _castingPoint.position;
        bullet.rotation = _castingPoint.rotation;
    }
    
    private void StartedCasting(InputAction.CallbackContext context)
    {
        _casting = true;
    }

    private void EndCast(InputAction.CallbackContext context)
    {
        _casting = false;
    }
}